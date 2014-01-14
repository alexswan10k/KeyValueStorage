using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.RetryStrategies;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using KeyValueStorage.Utility;

namespace KeyValueStorage.AzureTable
{
    public class AzureTableStoreProvider : IStoreProvider
    {
        public CloudTableClient client { get; protected set; }
        public KVSExpiredKeyCleaner KeyCleaner { get; protected set; } 
        public string KVSTableName { get; protected set;}
        const string KVSTableNameDefault = "KVS";
        const string LockPrefix = "-L-";

        public CloudTable Table
        {
            get
            {
                return client.GetTableReference(KVSTableName);
            }
        }

        public AzureTableStoreProvider(CloudStorageAccount storageAccount)
        {
            KVSTableName = KVSTableNameDefault;
            client = storageAccount.CreateCloudTableClient();
        }

        public AzureTableStoreProvider(CloudTableClient tableClient)
        {
            KVSTableName = KVSTableNameDefault;
            client = tableClient;
        }

        public AzureTableStoreProvider(CloudStorageAccount storageAccount, KVSExpiredKeyCleaner keyCleaner)
            :this(storageAccount)
        {
            KeyCleaner = keyCleaner;
        }

        public AzureTableStoreProvider(CloudTableClient tableClient, KVSExpiredKeyCleaner keyCleaner)
            :this(tableClient)
        {
            KeyCleaner = keyCleaner;
        }

        public bool SetupWorkingTable()
        {
            return Table.CreateIfNotExists();
        }

        protected KVEntity get(string key)
        {
            var query = new TableQuery<KVEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key));
            return Table.ExecuteQuery(query).FirstOrDefault();
        }

        #region IStoreProvider
        public void Initialize()
        {
            SetupWorkingTable();
        }

        public string Get(string key)
        {
            var entity = get(key);
            if(entity != null)
                return entity.Value;
            return null;
        }

        public void Set(string key, string value)
        {
            Table.Execute(TableOperation.InsertOrReplace(new KVEntity() { PartitionKey = key, RowKey = "1", Value = value, CAS = 1 }));
        }

        public void Remove(string key)
        {
            var item = get(key);

            if (item != null)
                Table.Execute(TableOperation.Delete(item));
        }

        public string Get(string key, out ulong cas)
        {
            cas = 0;
            var entity = get(key);
            if (entity != null)
            {
                cas = (ulong)entity.CAS;
                return entity.Value;
            }
            return null;
        }

        public void Set(string key, string value, ulong cas)
        {
            using (var keyLock = GetKeyLock(LockPrefix + key, DateTime.UtcNow.AddSeconds(10)))
            {
                var entity = get(key);
                if (entity != null)
                {
                    if (entity.CAS != (long)cas)
                        throw new CASException("CAS Expired");

                    entity.CAS++;
                    entity.Value = value;

                    Table.Execute(TableOperation.Replace(entity));
                }
                else
                    Table.Execute(TableOperation.Insert(new KVEntity() { PartitionKey = key, RowKey = "1", Value = value, CAS = (long)cas }));
            }
        }

        public void Set(string key, string value, DateTime expires)
        {
            Set(key, value);
            SetKeyExpiry(key, expires);
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            var expires = DateTime.UtcNow + expiresIn;

            Set(key, value);
            SetKeyExpiry(key, expires);
        }

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            Set(key, value, cas);
            SetKeyExpiry(key, expires);
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
        {
            var expires = DateTime.UtcNow + expiresIn;

            Set(key, value, cas);
            SetKeyExpiry(key, expires);
        }

        public bool Exists(string key)
        {
            var entity = get(key);
            if (entity != null)
                return true;
            return false;
        }

        public DateTime? ExpiresOn(string key)
        {
            if (KeyCleaner != null)
                return KeyCleaner.GetKeyExpiry(key);
            else
                throw new Exception("Cannot get expiry as no KeyCleaner is available");
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            var keyMax = Encoding.UTF8.GetString(ArrayHelpers.IncrementByteArrByOne(Encoding.UTF8.GetBytes(key)));

            var query = new TableQuery<KVEntity>().Where("PartitionKey ge '" + key + "' and PartitionKey lt '" + keyMax + "'");
            return Table.ExecuteQuery(query).Select(s => s.Value).ToList();
        }

        public IEnumerable<string> GetAllKeys()
        {
            var query = new TableQuery<KVEntity>();
            return Table.ExecuteQuery(query).Select(s => s.PartitionKey).ToList();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            var keyMax = Encoding.UTF8.GetString(ArrayHelpers.IncrementByteArrByOne(Encoding.UTF8.GetBytes(key)));

            var query = new TableQuery<KVEntity>().Where("PartitionKey ge '"+key + "' and PartitionKey lt '"+keyMax+"'");
            return Table.ExecuteQuery(query).Select(s => s.PartitionKey).ToList();
        }

        public int CountStartingWith(string key)
        {
            var keyMax = Encoding.UTF8.GetString(ArrayHelpers.IncrementByteArrByOne(Encoding.UTF8.GetBytes(key)));

            var query = new TableQuery<KVEntity>().Where("PartitionKey ge '"+key + "' and PartitionKey lt '"+keyMax+"'");
            return Table.ExecuteQuery(query).Count();
        }

        public int CountAll()
        {
            return Table.ExecuteQuery(new TableQuery<KVEntity>()).Count();
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            return getNextSequenceValue(key, increment, 0);
        }

        protected ulong getNextSequenceValue(string key, int increment, int tryCount)
        {
            return IStoreProviderInternalHelpers.GetNextSequenceValueViaCASWithRetries(this, key, increment, tryCount);
        }

        public void Append(string key, string value)
        {
            append(key, value);
        }

        public IRetryStrategy GetDefaultRetryStrategy()
        {
            return new SimpleRetryStrategy(5, 1000);
        }

		public IKeyLock GetKeyLock(string key, DateTime expires, IRetryStrategy retryStrategy = null, string workerId = null)
	    {
			return new KVSLockWithCAS(key, expires, this, retryStrategy ?? new SimpleLockRetryStrategy(5, 500), workerId);
	    }

	    private void append(string key, string value, int tryCount = 0)
        {
            try
            {
                ulong cas;
                var val = Get(key, out cas);

                Set(key, val + value);
            }
            catch (CASException casEx)
            {
                if (tryCount >= 10)
                    throw new Exception("Could not get sequence value", casEx);

                append(key, value, tryCount++);
            }
        }

        private void SetKeyExpiry(string key, DateTime expires)
        {
            if (KeyCleaner == null)
                throw new InvalidOperationException("Expiry date cannot be set if no key cleaner is present");

            KeyCleaner.SetKeyExpiry(key, expires);
        }
        #endregion

        public void Dispose()
        {
            //No azure client components are disposable... 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using KeyValueStorage.Utility;

namespace KeyValueStorage.AzureTable
{
    public class AzureTableStoreProvider : IStoreProvider
    {

        public static StoreExpiryManager StoreExpiryManager { get; protected set; }

        public CloudTableClient client { get; protected set; }
        public string KVSTableName { get; protected set;}
        const string KVSTableNameDefault = "KVS";

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
        public string Get(string key)
        {
            var entity = get(key);
            if(entity != null)
                return entity.Value;
            return null;
        }

        public void Set(string key, string value)
        {
            Table.Execute(TableOperation.InsertOrReplace(new KVEntity() { PartitionKey = key, RowKey = "1", Value = value }));
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

        public void Set(string key, string value, DateTime expires)
        {
            Table.Execute(TableOperation.InsertOrReplace(new KVEntity() { PartitionKey = key, RowKey = "1", Value = value, Expires = expires }));
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            Table.Execute(TableOperation.InsertOrReplace(new KVEntity() { PartitionKey = key, RowKey = "1", Value = value, Expires = DateTime.UtcNow + expiresIn }));
        }

        public void Set(string key, string value, ulong CAS, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong CAS, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
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
            var entity = get(key);
            if (entity != null)
                return entity.Expires;
            return null;
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            var keyMax = Encoding.UTF8.GetString(incrementByteArrByOne(Encoding.UTF8.GetBytes(key)));

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
            var keyMax = Encoding.UTF8.GetString(incrementByteArrByOne(Encoding.UTF8.GetBytes(key)));

            var query = new TableQuery<KVEntity>().Where("PartitionKey ge '"+key + "' and PartitionKey lt '"+keyMax+"'");
            return Table.ExecuteQuery(query).Select(s => s.PartitionKey).ToList();
        }

        public int CountStartingWith(string key)
        {
            var keyMax = Encoding.UTF8.GetString(incrementByteArrByOne(Encoding.UTF8.GetBytes(key)));

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
            try
            {
                ulong cas;
                var obj = Get(key, out cas);
                ulong seqVal;

                if (!ulong.TryParse(obj, out seqVal))
                {
                    seqVal = 0;
                }
                seqVal = seqVal + (ulong)increment;
                Set(key, seqVal.ToString(), cas);
                return seqVal;
            }
            catch (CASException casEx)
            {
                if (tryCount >= 10)
                    throw new Exception("Could not get sequence value", casEx);

                System.Threading.Thread.Sleep(20);
                //retry
                return getNextSequenceValue(key, increment,tryCount++);
            }
            return 0;
        }

        public void Append(string key, string value)
        {
            append(key, value);
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
        #endregion

        public void Dispose()
        {
            //No azure client components are disposable... 
        }

        private byte[] incrementByteArrByOne(byte[] arr)
        {
            byte[] outBytes = new byte[arr.Length];
            byte carry = 0;
            bool first = true;
            for(int i = arr.Length-1; i >= 0; i--)
            {
                byte procByte = 0;
                procByte = arr[i];
                if(first)
                {
                    if(procByte < byte.MaxValue)
                        procByte ++;

                }

                if (carry > 0)
                {
                    procByte += carry;
                    carry = 0;
                }

                if (procByte > byte.MaxValue)
                    carry = 1;

                first = false;
                outBytes[i] = procByte;
            }

            return outBytes;
        }
    }
}

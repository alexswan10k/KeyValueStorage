using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace KeyValueStorage.AzureTable
{
    public class AzureTableStoreProvider : IStoreProvider
    {
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
            Table.Execute(TableOperation.Delete(item));
        }

        public string Get(string key, out ulong cas)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public DateTime? ExpiresOn(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetContaining(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysContaining(string key)
        {
            throw new NotImplementedException();
        }

        public int CountStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountContaining(string key)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public long GetNextSequenceValue(string key, int increment)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            //No azure client components are disposable... 
        }
    }
}

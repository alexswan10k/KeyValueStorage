
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Cassandra
{
    public class CassandraStoreProvider : IStoreProvider
    {
        #region IStoreProvider
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public string Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
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

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
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

        public IEnumerable<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            throw new NotImplementedException();
        }

        public void Append(string key, string value)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

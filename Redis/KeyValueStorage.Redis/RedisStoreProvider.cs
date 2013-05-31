using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;
using ServiceStack.Redis;

namespace KeyValueStorage.Redis
{
    public class RedisStoreProvider : IStoreProvider
    {
        RedisClient Client;
        public RedisStoreProvider(RedisClient client)
        {
            Client = client;   
        }

        #region IStoreProvider
        public string Get(string key)
        {
            return Encoding.UTF8.GetString(Client.Get(key));
        }

        public void Set(string key, string value)
        {
            Client.Set(key, Encoding.UTF8.GetBytes(value));
        }

        public void Delete(string key)
        {
            Client.Remove(key);
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
            Client.Set(key, value, expires);
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            Client.Set(key, value, DateTime.UtcNow + expiresIn);
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
            return Client.Exists(key) == 1? true: false;
        }

        public DateTime ExpiresOn(string Key)
        {
            //looking for implementation
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            return Client.Keys(key + "*").Select(s => s[1]).Cast<string>();
        }

        public IEnumerable<string> GetContaining(string key)
        {
            return Client.Keys("*" + key + "*").Select(s => s[1]).Cast<string>();
        }

        public IEnumerable<string> GetAllKeys()
        {
            return Client.GetAllKeys();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            return Client.SearchKeys(key + "*");
        }

        public IEnumerable<string> GetKeysContaining(string key)
        {
            return Client.SearchKeys("*" + key + "*");
        }

        public int CountStartingWith(string key)
        {
            return Client.SearchKeys(key + "*").Count;
        }

        public int CountContaining(string key)
        {
            return Client.SearchKeys("*" + key + "*").Count;
        }

        public int CountAll()
        {
            return Client.GetAllKeys().Count;
        }

        public long GetNextSequenceValue(string key, int increment)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}

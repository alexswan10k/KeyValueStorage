using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using ServiceStack.Redis;

namespace KeyValueStorage.Redis
{
    public class RedisStoreProvider : IStoreProvider
    {
        RedisClient Client;
        const string casSuffix = "-CAS";
        const string lockSuffix = "-L";

        public RedisStoreProvider(RedisClient client)
        {
            Client = client;   
        }

        #region IStoreProvider
        public string Get(string key)
        {
            var res = Client.Get(key);

            if(res != null)
                return Encoding.UTF8.GetString(res);
            return string.Empty;
        }

        public void Set(string key, string value)
        {
            Client.Set(key, Encoding.UTF8.GetBytes(value));
        }

        public void Remove(string key)
        {
            Client.Remove(key);
        }

        public string Get(string key, out ulong cas)
        {
            cas = Client.Get<ulong>(key + casSuffix);
            return this.Get(key);
        }

        public void Set(string key, string value, ulong cas)
        {
            using (var casLock = Client.AcquireLock(key + lockSuffix, TimeSpan.FromSeconds(10)))
            {
                var casDB = Client.Get<ulong>(key + casSuffix);

                if (casDB != cas)
                    throw new CASException("CAS values do not match");

                Client.Set(key, Encoding.UTF8.GetBytes(value));
                GetNextSequenceValue(key + casSuffix, 1);
            }
        }

        public void Set(string key, string value, DateTime expires)
        {
            Client.Set(key, Encoding.UTF8.GetBytes(value), expires);
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            Client.Set(key, Encoding.UTF8.GetBytes(value), DateTime.UtcNow + expiresIn);
        }

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            using (var casLock = Client.AcquireLock(key + casSuffix))
            {
                var casDB = Client.Get<ulong>(key + casSuffix);

                if (casDB != cas)
                    throw new Exception("CAS values do not match");

                Client.Set(key, Encoding.UTF8.GetBytes(value), expires);
                GetNextSequenceValue(key + casSuffix, 1);
            }
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
        {
            using (var casLock = Client.AcquireLock(key + casSuffix))
            {
                var casDB = Client.Get<ulong>(key + casSuffix);

                if (casDB != cas)
                    throw new CASException("CAS expired");

                Client.Set(key, Encoding.UTF8.GetBytes(value), DateTime.UtcNow + expiresIn);
                GetNextSequenceValue(key + casSuffix, 1);
            }
        }

        public bool Exists(string key)
        {
            return Client.Exists(key) == 1? true: false;
        }

        public DateTime? ExpiresOn(string key)
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

        public ulong GetNextSequenceValue(string key, int increment)
        {
            ulong seq = 0;

            for (int i = 0; i < increment; i++)
            {
                seq = (ulong)Client.Incr(key);
            }

            return seq;
        }
        #endregion

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}

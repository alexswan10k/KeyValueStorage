using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;
using Couchbase;

namespace KeyValueStorage.Couchbase
{
    public class CouchbaseStoreProvider : IStoreProvider
    {
        CouchbaseClient Client;
        public CouchbaseStoreProvider(CouchbaseClient client)
        {
            Client = client;
        }

        public string Get(string key)
        {
            return (string)Client.Get(key);
        }

        public void Set(string key, string value)
        {
            Client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value);
        }

        public void Remove(string key)
        {
            Client.Remove(key);
        }

        public string Get(string key, out ulong cas)
        {
            var casRes = Client.GetWithCas(key);
            cas = casRes.Cas;
            return (string)casRes.Result;
        }

        public void Set(string key, string value, ulong cas)
        {
            var casRes = Client.Cas(Enyim.Caching.Memcached.StoreMode.Replace, key, value, cas);

            if (casRes.Result == false)
                throw new Exception("cas expired");
        }

        public void Set(string key, string value, DateTime expires)
        {
            Client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value, expires);
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            Client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value,DateTime.UtcNow + expiresIn);
        }

        public void Set(string key, string value, ulong CAS, DateTime expires)
        {
            Client.Cas(Enyim.Caching.Memcached.StoreMode.Set, key, value, expires, CAS);
        }

        public void Set(string key, string value, ulong CAS, TimeSpan expiresIn)
        {
            Client.Cas(Enyim.Caching.Memcached.StoreMode.Set, key, value, DateTime.UtcNow + expiresIn, CAS);
        }

        public bool Exists(string key)
        {
            return Client.KeyExists(key);
        }

        public DateTime? ExpiresOn(string key)
        {
            //Couchbase does not seem to have an easy api to support this
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetContaining(string key)
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllKeys()
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysContaining(string key)
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public int CountStartingWith(string key)
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public int CountContaining(string key)
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            //Requires views?
            throw new NotImplementedException();
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            return Client.Increment(key, 1, (ulong)increment);
        }

        public void Dispose()
        {
            //we do not dispose of the client as its lifetime is intended to extend beyond the lifetime of the provider and context.
        }
    }
}

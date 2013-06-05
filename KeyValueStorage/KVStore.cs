using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage
{
    public class KVStore : IKVStore
    {
        public static Factory Factory { get; set; }

        public static void Initialize(Func<IStoreProvider> createProviderDel)
        {
            Factory = new Factory(createProviderDel);
        }

        public static void Initialize(Func<IStoreProvider> createProviderDel, ITextSerializer serializer)
        {
            Factory = new Factory(createProviderDel, serializer);
        }

        public KVStore(IStoreProvider provider)
        {
            StoreProvider = provider;
            Serializer = new ServiceStackTextSerializer();
        }

        public KVStore(IStoreProvider provider, ITextSerializer serializer)
        {
            StoreProvider = provider;
            Serializer = serializer;
        }

        
        public IStoreProvider StoreProvider { get; protected set; }
        public ITextSerializer Serializer { get; protected set; }

        #region IKeyValueStore
        public T Get<T>(string key)
        {
            return Serializer.Deserialize<T>(StoreProvider.Get(key));
        }

        public void Set<T>(string key, T value)
        {
            StoreProvider.Set(key, Serializer.Serialize(value));
        }

        public void Delete(string key)
        {
            StoreProvider.Remove(key);
        }

        public T Get<T>(string key, out ulong cas)
        {
            return Serializer.Deserialize<T>(StoreProvider.Get(key, out cas));
        }

        public void Set<T>(string key, T value, ulong cas)
        {
            StoreProvider.Set(key, Serializer.Serialize(value), cas);
        }

        public void Set<T>(string key, T value, DateTime expires)
        {
            StoreProvider.Set(key, Serializer.Serialize(value), expires);
        }

        public void Set<T>(string key, T value, TimeSpan expiresIn)
        {
            StoreProvider.Set(key, Serializer.Serialize(value), expiresIn);
        }

        public void Set<T>(string key, T value, ulong cas, DateTime expires)
        {
            StoreProvider.Set(key, Serializer.Serialize(value), cas, expires);
        }

        public void Set<T>(string key, T value, ulong cas, TimeSpan expiresIn)
        {
            StoreProvider.Set(key, Serializer.Serialize(value), cas, expiresIn);
        }


        public bool Exists(string key)
        {
            return StoreProvider.Exists(key);
        }

        public DateTime? ExpiresOn(string key)
        {
            return StoreProvider.ExpiresOn(key);
        }

        #region Queries
        public IEnumerable<T> GetStartingWith<T>(string key)
        {
            return StoreProvider.GetStartingWith(key).Select(s => Serializer.Deserialize<T>(s));
        }

        public IEnumerable<string> GetAllKeys()
        {
            return StoreProvider.GetAllKeys();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            return StoreProvider.GetKeysStartingWith(key);
        }
        #endregion

        #region Scalar Queries
        public int CountStartingWith(string key)
        {
            return StoreProvider.CountStartingWith(key);
        }

        public int CountContaining(string key)
        {
            return StoreProvider.CountContaining(key);
        }

        public int CountAll()
        {
            return StoreProvider.CountAll();
        }
        #endregion

        #region Sequences
        public ulong GetNextSequenceValue(string key)
        {
            return StoreProvider.GetNextSequenceValue(key, 1);
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            return StoreProvider.GetNextSequenceValue(key, increment);
        }
        #endregion
        #endregion

        public void Dispose()
        {
            //todo: implement the dispose pattern
            StoreProvider.Dispose();
        }
    }
}

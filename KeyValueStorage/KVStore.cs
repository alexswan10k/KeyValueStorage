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

        #region CollectionOperations
        public IEnumerable<T> GetCollection<T>(string key)
        {
            return separateJsonArray(StoreProvider.Get(key)).Select(s => Serializer.Deserialize<T>(s));
        }

        public IEnumerable<T> GetCollection<T>(string key, out ulong cas)
        {
            return separateJsonArray(StoreProvider.Get(key, out cas)).Select(s => Serializer.Deserialize<T>(s));
        }

        public void SetCollection<T>(string key, IEnumerable<T> values)
        {
            StoreProvider.Set(key, String.Concat(values.Select(s => Serializer.Serialize(s))));
        }

        public void SetCollection<T>(string key, IEnumerable<T> values, ulong cas)
        {
            StoreProvider.Set(key, String.Concat(values.Select(s => Serializer.Serialize(s))), cas);
        }

        public void AppendToCollection<T>(string key, T value)
        {
            StoreProvider.Append(key, Serializer.Serialize(value));
        }

        public void RemoveFromCollection<T>(string key, T value)
        {
            ulong cas;
            var collection = GetCollection<T>(key, out cas).ToList();
            var itemToRemove = collection.SingleOrDefault(q => q.Equals(value));
            collection.Remove(itemToRemove);
            SetCollection(key, collection);
        }
        #endregion

        private IEnumerable<string> separateJsonArray(string json)
        {
            int depth = 0;
            List<string> outputStringEnumerable = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '{')
                    depth++;
                else if (json[i] == '}')
                    depth--;
                else if (depth < 0)
                    throw new Exception("Json is invalid");

                if (depth == 0 && i > 0)
                {
                    outputStringEnumerable.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                else
                    sb.Append(json[i]);
            }

            return outputStringEnumerable;
        }

        public void Dispose()
        {
            //todo: implement the dispose pattern
            StoreProvider.Dispose();
        }
    }
}

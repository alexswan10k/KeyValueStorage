using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.RetryStrategies;
using KeyValueStorage.Utility;

namespace KeyValueStorage
{
    public class KVStore : IKVStore
    {
        private readonly IRetryStrategy _retryStrategy;
        public static Factory Factory { get; set; }

        public static void Initialize(Func<IStoreProvider> createProviderDel, ITextSerializer serializer = null, IRetryStrategy retryStrategy = null)
        {
            Factory = new Factory(createProviderDel, serializer, retryStrategy);
        }

        public KVStore(IStoreProvider provider, ITextSerializer serializer = null, IRetryStrategy retryStrategy = null)
        {
            if(provider == null)
                throw new ArgumentException("Provider cannot be null", "provider");

            _retryStrategy = retryStrategy ?? provider.GetDefaultRetryStrategy();
            StoreProvider = provider;
            Serializer = serializer ?? new ServiceStackTextSerializer();
        }

        
        public IStoreProvider StoreProvider { get; protected set; }
        public ITextSerializer Serializer { get; protected set; }

        #region IKeyValueStore
        public T Get<T>(Key key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => Serializer.Deserialize<T>(StoreProvider.Get(key)));
        }

        public void Set<T>(Key key, T value)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Set(key, Serializer.Serialize(value)));
        }

        public void Delete(Key key)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Remove(key));
        }

        public T Get<T>(Key key, out ulong cas)
        {
            //horrendous hack for accomodating out params!
            ulong casOut = 0;

            var outerResult =  _retryStrategy.ExecuteFuncWithRetry(
                () =>
                    {
                        ulong cas2;
                        var innerResult = Serializer.Deserialize<T>(StoreProvider.Get(key, out cas2));
                        casOut = cas2;
                        return innerResult;
                    });

            cas = casOut;
            return outerResult;
        }


        public void Set<T>(Key key, T value, ulong cas)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Set(key, Serializer.Serialize(value), cas));
        }

        public void Set<T>(Key key, T value, DateTime expires)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Set(key, Serializer.Serialize(value), expires));
        }

        public void Set<T>(Key key, T value, TimeSpan expiresIn)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Set(key, Serializer.Serialize(value), expiresIn));
        }

        public void Set<T>(Key key, T value, ulong cas, DateTime expires)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Set(key, Serializer.Serialize(value), cas, expires));
        }

        public void Set<T>(Key key, T value, ulong cas, TimeSpan expiresIn)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => StoreProvider.Set(key, Serializer.Serialize(value), cas, expiresIn));
        }


        public bool Exists(string key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.Exists(key));
        }

        public DateTime? ExpiresOn(string key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.ExpiresOn(key));
        }

        #region Queries
        public IEnumerable<T> GetStartingWith<T>(Key key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.GetStartingWith(key).Select(s => Serializer.Deserialize<T>(s)).ToList()
            );
        }

        public IEnumerable<Key> GetAllKeys()
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.GetAllKeys()).Select(s => new Key(s));
        }

        public IEnumerable<Key> GetKeysStartingWith(Key key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.GetKeysStartingWith(key)).Select(s => new Key(s));
        }
        #endregion

        #region Scalar Queries
        public int CountStartingWith(Key key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.CountStartingWith(key));
        }

        public int CountAll()
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.CountAll());
        }
        #endregion

        #region Sequences
        public ulong GetNextSequenceValue(Key key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.GetNextSequenceValue(key, 1));
        }

        public ulong GetNextSequenceValue(Key key, int increment)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () => StoreProvider.GetNextSequenceValue(key, increment));
        }
        #endregion
        #endregion

        #region CollectionOperations
        public IEnumerable<T> GetCollection<T>(Key key)
        {
            return _retryStrategy.ExecuteFuncWithRetry(
                () =>
                    Helpers.SeparateJsonArray(StoreProvider.Get(key)).Select(s => Serializer.Deserialize<T>(s)).ToList()
                );
        }

        public IEnumerable<T> GetCollection<T>(Key key, out ulong cas)
        {
            // nasty hack for out params
            ulong casOut = 0;

            var outerResult = _retryStrategy.ExecuteFuncWithRetry(
                () =>
                    {
                        ulong cas2;
                        var innerResult =
                            Helpers.SeparateJsonArray(StoreProvider.Get(key, out cas2)).Select(
                                s => Serializer.Deserialize<T>(s)).ToList();
                        casOut = cas2;
                        return innerResult;
                    });

            cas = casOut;
            return outerResult;
        }

        public void SetCollection<T>(Key key, IEnumerable<T> values)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () =>
                    StoreProvider.Set(key, String.Concat(values.Select(s => Serializer.Serialize(s))))
                );
        }

        public void SetCollection<T>(Key key, IEnumerable<T> values, ulong cas)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () => 
                    StoreProvider.Set(key, String.Concat(values.Select(s => Serializer.Serialize(s))), cas)
                );
        }

        public void AppendToCollection<T>(Key key, T value)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () =>
                    StoreProvider.Append(key, Serializer.Serialize(value))
                );
        }

        public void RemoveFromCollection<T>(Key key, T value)
        {
            _retryStrategy.ExecuteDelegateWithRetry(
                () =>
                    {
                        ulong cas;
                        var collection = GetCollection<T>(key, out cas).ToList();
                        var itemToRemove = collection.SingleOrDefault(q => q.Equals(value));
                        collection.Remove(itemToRemove);
                        SetCollection(key, collection);
                    });
        }
        #endregion

        public void Dispose()
        {
            //todo: implement the dispose pattern
            StoreProvider.Dispose();
        }
    }
}

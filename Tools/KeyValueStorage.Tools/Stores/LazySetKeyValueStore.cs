using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools.Stores
{
    public class LazySetKeyValueStore : IKVStore
    {
        private IKVStore _store;
        private Queue<Action> _writeActions = new Queue<Action>(); 

        public LazySetKeyValueStore(IKVStore underlyingStore)
        {
            _store = underlyingStore;
        }
        public void Dispose()
        {
            _store.Dispose();
        }

        public IStoreProvider StoreProvider { get { return _store.StoreProvider; } }
        public ITextSerializer Serializer { get { return _store.Serializer; } }

        public T Get<T>(string key)
        {
            return _store.Get<T>(key);
        }

        public void Set<T>(string key, T value)
        {
            _writeActions.Enqueue(() =>_store.Set(key, value));
        }

        public void Delete(string key)
        {
            _writeActions.Enqueue(() => _store.Delete(key));       
        }

        public T Get<T>(string key, out ulong cas)
        {
            return _store.Get<T>(key, out cas);
        }

        public void Set<T>(string key, T value, ulong cas)
        {
            _writeActions.Enqueue(() => _store.Set(key, value, cas));    
        }

        public void Set<T>(string key, T value, DateTime expires)
        {
            _writeActions.Enqueue(() => _store.Set(key, value, expires));    
        }

        public void Set<T>(string key, T value, TimeSpan expiresIn)
        {
            _writeActions.Enqueue(() => _store.Set(key, value, expiresIn));    
        }

        public void Set<T>(string key, T value, ulong CAS, DateTime expires)
        {
            _writeActions.Enqueue(() => _store.Set(key, value, expires));    
        }

        public void Set<T>(string key, T value, ulong CAS, TimeSpan expiresIn)
        {
            _writeActions.Enqueue(() => _store.Set(key, value, expiresIn));    
        }

        public bool Exists(string key)
        {
            return _store.Exists(key);
        }

        public DateTime? ExpiresOn(string key)
        {
            return _store.ExpiresOn(key);
        }

        public IEnumerable<T> GetStartingWith<T>(string key)
        {
            return _store.GetStartingWith<T>(key);
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _store.GetAllKeys();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            return _store.GetKeysStartingWith(key);
        }

        public int CountStartingWith(string key)
        {
            return _store.CountStartingWith(key);
        }

        public int CountAll()
        {
            return _store.CountAll();
        }

        public ulong GetNextSequenceValue(string key)
        {
            return _store.GetNextSequenceValue(key);
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            return _store.GetNextSequenceValue(key, increment);
        }

        public IEnumerable<T> GetCollection<T>(string key)
        {
            return _store.GetCollection<T>(key);
        }

        public IEnumerable<T> GetCollection<T>(string key, out ulong cas)
        {
            return _store.GetCollection<T>(key, out cas);
        }

        public void SetCollection<T>(string key, IEnumerable<T> values)
        {
            _writeActions.Enqueue(() => _store.SetCollection(key, values));    
        }

        public void SetCollection<T>(string key, IEnumerable<T> values, ulong cas)
        {
            _writeActions.Enqueue(() => _store.SetCollection(key, values, cas));    
        }

        public void AppendToCollection<T>(string key, T value)
        {
            _writeActions.Enqueue(() => _store.AppendToCollection(key, value));    
        }

        public void SaveChanges()
        {
            while (_writeActions.Count > 0)
            {
                _writeActions.Dequeue().Invoke();
            }
        }
    }
}

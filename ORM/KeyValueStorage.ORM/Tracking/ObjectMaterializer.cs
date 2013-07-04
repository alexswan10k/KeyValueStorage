using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.ORM.Tracking
{
    /// <summary>
    /// Controls and mediates materialization of objects from the underlying store.
    /// Is also responsible for attaching all loaded objects to the tracker if they are not handled, and 
    /// </summary>
    public class ObjectMaterializer : IDisposable
    {
        public ObjectMaterializer(ObjectTracker tracker)
        {
            Store = KVStore.Factory.Get();
            ObjectTracker = tracker;
            StoreCache = new ConcurrentDictionary<string, object>();
        }

        public ObjectTracker ObjectTracker { get; private set; }
        public IKVStore Store { get; private set; }
        public ConcurrentDictionary<string, object> StoreCache { get; private set; }

        public T GetObject<T>(string key) where T:class
        {
            object value;
            if (StoreCache.TryGetValue(key, out value))
            {
                if (value is T)
                    return value as T;
            }

            var valueTyped = Store.Get<T>(key);
            StoreCache.TryAdd(key, valueTyped);
            ObjectTracker.AttachObject(valueTyped);

            return valueTyped;
        }

        public IEnumerable<T> GetStartingWith<T>(string startOfKey)
        {
            return Store.GetStartingWith<T>(startOfKey);
        }

        public void Dispose()
        {
            Store.Dispose();
        }
    }
}

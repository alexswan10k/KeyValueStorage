using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage
{
    public static class IKVStoreExtensions
    {
        public static void RemoveFromCollection<T>(this IKVStore store, string key, T value)
        {
            removeFromCollection(store, key, value, 0);
        }

        private static void removeFromCollection<T>(IKVStore store, string key, T value, int tryCount)
        {
            try
            {
                ulong cas;
                var collection = store.GetCollection<T>(key, out cas).ToList();
                var itemToRemove = collection.SingleOrDefault(q => q.Equals(value));
                collection.Remove(itemToRemove);
                store.SetCollection(key, collection, cas);
            }
            catch (CASException casEx)
            {
                if (tryCount >= 10)
                    throw new Exception("Could not get sequence value", casEx);

                System.Threading.Thread.Sleep(20);

                removeFromCollection(store, key, value, tryCount++);
            }
        }
    }
}

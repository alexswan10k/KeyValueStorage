using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using ServiceStack.Text;

namespace KeyValueStorage.ORM
{
    public abstract class KVSDbSet : IEnumerable
    {
        public string BaseKey { get; set; }
        public IKVStore Store { get; set; }
        public const string CollectionPrefix = ":C:";
        public const string SequenceSuffix = ":S";
    }

    public class KVSDbSet<T> : KVSDbSet, ICollection<T> where T : class
    {
        public IDictionary<ulong, T> CachedCollection { get; protected set; }
        public bool CachedIsFullList { get; protected set; }

        public KVSDbSet()
        {
            CachedCollection = new Dictionary<ulong, T>();
        }

        protected void LoadIfNotLoaded()
        {
            if (!CachedIsFullList)
                RefreshCachedCollection();
        }

        protected void RefreshCachedCollection()
        {
            var keys = Store.GetKeysStartingWith(BaseKey + CollectionPrefix);
            var collection = Store.GetStartingWith<T>(BaseKey + CollectionPrefix);

            if (keys.Count() != collection.Count())
                throw new InvalidProgramException("Keys returned and collection do not match");

            CachedCollection.Clear();

            for (int i = 0; i < keys.Count(); i++)
            {
                var keyIdx = ulong.Parse(keys.ElementAt(i).Split(':').Last());
                CachedCollection.Add(keyIdx, collection.ElementAt(i));
            }

            if (CollectionRefreshed != null)
                CollectionRefreshed.Invoke(this, CachedCollection);
        }

        protected ulong GetNextSequenceValue()
        {
            return Store.GetNextSequenceValue(BaseKey + SequenceSuffix);
        }

        public void Add(T item)
        {
            var seqVal =GetNextSequenceValue();
            Store.Set(BaseKey + CollectionPrefix + seqVal, item);
            CachedCollection.Add(seqVal, item);
        }

        public void Clear()
        {
            if (!CachedIsFullList)
                LoadIfNotLoaded();

            foreach (var key in CachedCollection.Keys)
            {
                Store.Delete(BaseKey + CollectionPrefix + key.ToString());
            }

            CachedCollection.Clear();
        }

        public bool Contains(T item)
        {
            if (CachedCollection.Values.Contains(item))
                return true;
            else if(CachedCollection.Select( s=> s.Value.Dump()).Contains(item.Dump()))
                return true;

            RefreshCachedCollection();

            if (CachedCollection.Values.Contains(item))
                return true;
            else if (CachedCollection.Select(s => s.Value.Dump()).Contains(item.Dump()))
                return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < arrayIndex + this.Count; i++)
            {
                array[i] = this.ElementAt(i - arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                if (!CachedIsFullList)
                    LoadIfNotLoaded();

                return CachedCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (CachedCollection.Values.Contains(item))
            {
                var key = CachedCollection.First(q => q.Value == item).Key;
                Store.Delete(BaseKey + CollectionPrefix + key.ToString());
                CachedCollection.Remove(key);
                return true;
            }
            else if (CachedCollection.Select(s => s.Value.Dump()).Contains(item.Dump()))
            {
                var key = CachedCollection.First(q => q.Value.Dump() == item.Dump()).Key;
                Store.Delete(BaseKey + CollectionPrefix + key.ToString());
                CachedCollection.Remove(key);
            }

            RefreshCachedCollection();

            if (CachedCollection.Values.Contains(item))
            {
                var key = CachedCollection.First(q => q.Value.Dump() == item.Dump()).Key;
                Store.Delete(BaseKey + CollectionPrefix + key.ToString());
                CachedCollection.Remove(key);
            }
            else if (CachedCollection.Select(s => s.Value.Dump()).Contains(item.Dump()))
            {
                var key = CachedCollection.First(q => q.Value.Dump() == item.Dump()).Key;
                Store.Delete(BaseKey + CollectionPrefix + key.ToString());
                CachedCollection.Remove(key);
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!CachedIsFullList)
                LoadIfNotLoaded();

            return CachedCollection.Values.GetEnumerator();
        }

        public T GetById(ulong id)
        {
            return Store.Get<T>(BaseKey + CollectionPrefix + id);
        }

        public void Save(ulong id, T value)
        {
            Store.Set(BaseKey + CollectionPrefix + id.ToString(), value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!CachedIsFullList)
                LoadIfNotLoaded();

            return CachedCollection.Values.GetEnumerator();
        }

        public event EventHandler<IDictionary<ulong, T>> CollectionRefreshed;
        public event EventHandler<KeyValuePair<ulong, T>> ItemAdded;
        public event EventHandler<KeyValuePair<ulong, T>> ItemRemoved;
    }
}

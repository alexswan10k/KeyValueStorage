using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.ORM.Mapping;

namespace KeyValueStorage.ORM
{
    public class KVSCollection<T> : ICollection<T>
    {
        public const string FKCollectionSeparator = ":F:";

        public string BaseKey { get; set; }
        public IStoreProvider Provider { get; set; }
        public RelationshipMap Map { get; protected set; }
        protected KVSDbSet<T> ForeignCollection { get; set; }
        protected KVSDbSet LocalCollection { get; set; }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.ORM.Mapping;

namespace KeyValueStorage.ORM
{
    public class KVSCollection<T> : ICollection<T> where T : class
    {
        public const string FKCollectionSeparator = ":F:";

        public string BaseKey { get; set; }
        public IKVStore Store { get; set; }
        public RelationshipMap Map { get; protected set; }
        protected KVSDbSet<T> ForeignCollection { get; set; }
        protected KVSDbSet LocalCollection { get; set; }

        public string FKString
        {
            get
            {
                return BaseKey + FKCollectionSeparator + ForeignCollection.BaseKey; 
            }
        }

        public void Add(T item)
        {
            ForeignCollection.Add(item);
            Store.AppendToCollection(FKString, item);
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

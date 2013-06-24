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
        public Relationship Relationship { get; protected set; }

        public string FKString
        {
            get
            {
                return BaseKey + FKCollectionSeparator + Relationship.TargetDbSet.BaseKey; 
            }
        }

        public void Add(T item)
        {
            Relationship.TargetDbSet.Add(item);
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

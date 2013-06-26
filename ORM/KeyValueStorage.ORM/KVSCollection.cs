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
        public ContextBase Context { get; protected set; }
        public Relationship Rel { get; protected set; }

        public ICollection<T> InternalCollection { get; protected set; }
        public ICollection<ulong> InternalKeys { get; set; }
        public ICollection<T> ItemsAdded { get; protected set; }
        public ICollection<T> ItemsDeleted { get; protected set; }
        public bool IsAttached { get; protected set; }
        public bool IsLoaded { get; protected set; }


        public KVSCollection()
        {
            InternalCollection = new List<T>();
            ItemsAdded = new List<T>();
            ItemsDeleted = new List<T>();
        }

        public void Add(T item)
        {
            if (IsAttached)
            {
                var trackingInfo = Context.ObjectTracker.GetObjectTrackingInfo(item);

                if(trackingInfo == null)
                    Rel.TargetDbSet.Add(item);

                ItemsAdded.Add(item);
            }
            else
            {
                InternalCollection.Add(item);
            }
        }

        public void Clear()
        {
            InternalCollection.Clear();
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

        public void Load()
        {
            if (!IsAttached)
                throw new InvalidOperationException("Cannot load as collection is not attached");

            else
            {
                this.InternalCollection.Clear();

                foreach (var key in InternalKeys)
                {
                    var existingKeys = this.InternalCollection
                        .Select(s => this.Rel.RelationshipMap.TargetObjectMap.ThisKeyGetter.Invoke(s, new object[] { }))
                        .ToList();

                    if (!existingKeys.Contains(key))
                        this.InternalCollection.Add(this.Rel.TargetDbSet.GetByIdWeak(key) as T);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!IsAttached)
                return InternalCollection.GetEnumerator();

            if (!IsLoaded)
                Load();

            return InternalCollection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

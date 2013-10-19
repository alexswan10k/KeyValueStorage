using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public class KVForeignKeyMemoryRelationshipProvider : IKVForeignKeyRelationshipProvider
    {
        private readonly IList<Tuple<IRelationalKey, IRelationalKey>> _relationships;

        public KVForeignKeyMemoryRelationshipProvider(IList<Tuple<IRelationalKey, IRelationalKey>> relationships = null)
        {
            _relationships = relationships ?? new List<Tuple<IRelationalKey, IRelationalKey>>();
        }

        public IList<Tuple<IRelationalKey, IRelationalKey>> Relationships
        {
            get { return _relationships; }
        }

        public void Add(IRelationalKey key, IRelationalKey p1)
        {
            var item = _relationships.SingleOrDefault(q => q.Item1 == key && q.Item2 == p1);
            if (item == null)
                _relationships.Add(new Tuple<IRelationalKey, IRelationalKey>(key, p1));
        }

        public void Remove(IRelationalKey key, IRelationalKey p1)
        {
            var item = _relationships.SingleOrDefault(q => q.Item1 == key && q.Item2 == p1);
            if (item != null)
                _relationships.Remove(item);
        }

        public IEnumerable<KeyWithRelationship> GetRelationships(IRelationalKey key)
        {
            return _relationships.Where(q=> q.Item1 == key)
                .Select(s => new KeyWithRelationship(s.Item2, this));
        }

        public IEnumerable<IRelationalKey> GetKeys(IRelationalKey key)
        {
            return _relationships.Where(q => q.Item1 == key)
                .Select(s => s.Item2);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public class KVForeignKeyMemoryRelationshipProvider : IKVForeignKeyRelationshipProvider
    {
        private readonly IList<Tuple<Key, Key>> _relationships;

        public KVForeignKeyMemoryRelationshipProvider(IList<Tuple<Key, Key>> relationships = null)
        {
            _relationships = relationships ?? new List<Tuple<Key, Key>>();
        }

        public IList<Tuple<Key, Key>> Relationships
        {
            get { return _relationships; }
        }

        public void Add(Key key, Key p1)
        {
            var item = _relationships.SingleOrDefault(q => q.Item1 == key && q.Item2 == p1);
            if (item == null)
                _relationships.Add(new Tuple<Key, Key>(key, p1));
        }

        public void Remove(Key key, Key p1)
        {
            var item = _relationships.SingleOrDefault(q => q.Item1 == key && q.Item2 == p1);
            if (item != null)
                _relationships.Remove(item);
        }

        public IEnumerable<KeyWithRelationship> GetRelationships(Key key)
        {
            return _relationships.Where(q=> q.Item1 == key)
                .Select(s => new KeyWithRelationship(s.Item2, this));
        }

        public IEnumerable<Key> GetKeys(Key key)
        {
            return _relationships.Where(q => q.Item1 == key)
                .Select(s => s.Item2);
        }
    }
}
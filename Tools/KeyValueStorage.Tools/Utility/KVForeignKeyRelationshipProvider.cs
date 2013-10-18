using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Stores;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools.Utility
{
    public interface IKVForeignKeyRelationshipProvider
    {
        void Add(string key, string p1);
        void Remove(string key, string p1);
        IEnumerable<KeyWithRelationship> GetRelationships(string key);
        IEnumerable<string> GetKeys(string key);
    }

    public class KVForeignKeyRelationshipProvider : IKVForeignKeyRelationshipProvider
    {
        private readonly IKVStore _store;

        public KVForeignKeyRelationshipProvider(IKVStore store, string relationshipSuffix = ":R")
        {
            _store = new KeyTransformKVStore(store, new SuffixTransformer(relationshipSuffix));
        }

        public void Add(string key, string p1)
        {
            _store.AppendToCollection(key, p1);
        }

        public void Remove(string key, string p1)
        {
            _store.RemoveFromCollection(key, p1);
        }

        public IEnumerable<KeyWithRelationship> GetRelationships(string key)
        {
            return _store.GetCollection<string>(key)
                .Select(s => new KeyWithRelationship(s, this));
        }

        public IEnumerable<string> GetKeys(string key)
        {
            return _store.GetCollection<string>(key);
        }
    }
}
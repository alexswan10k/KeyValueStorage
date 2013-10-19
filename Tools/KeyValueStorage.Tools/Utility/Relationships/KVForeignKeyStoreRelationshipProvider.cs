using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Stores;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public class KVForeignKeyStoreRelationshipProvider : IKVForeignKeyRelationshipProvider
    {
        private readonly IKVStore _store;
        public const string relationshipSuffix = ":R:";

        /// <summary>
        /// Maintains a relationship of a key to a number of 'foreign key references' by creating an extra tracker key defined by itself and the suffix
        /// </summary>
        /// <param name="store">The underlying store to read to and write from</param>
        /// <param name="relationshipSuffixCollection">If there are more than 1 conceptual foreign key references, 
        /// this can be used to isolate the target collection types, so links to multiple conceptual 'tables' are not confused</param>
        /// <param name="relationshipSuffix">This will be used as a suffix in the tracker key so it is uniquely identifiable back to the source key. Normally the default is sufficient</param>
        public KVForeignKeyStoreRelationshipProvider(IKVStore store, string relationshipSuffixCollection = null)
        {
            _store = new KeyTransformKVStore(store, new SuffixTransformer(relationshipSuffix + relationshipSuffixCollection));
        }

        public void Add(IRelationalKey key, IRelationalKey p1)
        {
            _store.AppendToCollection(key.Value, p1);
        }

        public void Remove(IRelationalKey key, IRelationalKey p1)
        {
            _store.RemoveFromCollection(key.Value, p1);
        }

        public IEnumerable<KeyWithRelationship> GetRelationships(IRelationalKey key)
        {
            return _store.GetCollection<string>(key.Value).Select(s => new RelationalKey(s))
                .Select(s => new KeyWithRelationship(s, this));
        }

        public IEnumerable<IRelationalKey> GetKeys(IRelationalKey key)
        {
            return _store.GetCollection<IRelationalKey>(key.Value);
        }
    }
}
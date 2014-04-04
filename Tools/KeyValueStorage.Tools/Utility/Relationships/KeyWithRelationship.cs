using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using System.Linq;
using KeyValueStorage.Tools.Structured.Schema;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public class KeyWithRelationship
    {
        private readonly Key _key;
        private readonly IKVForeignKeyRelationshipProvider _kvLocalRelationshipProvider;

        public KeyWithRelationship(Key key, IKVForeignKeyRelationshipProvider kvLocalRelationshipProvider)
        {
            _key = key;
            _kvLocalRelationshipProvider = kvLocalRelationshipProvider;
        }

        public Key Key
        {
            get { return _key; }
        }

        public IKVForeignKeyRelationshipProvider KvLocalRelationshipProvider
        {
            get { return _kvLocalRelationshipProvider; }
        }

        public void Add(Key foreignKey, IKVForeignKeyRelationshipProvider foreignKeyReturnRelationshipProvider)
        {
            KvLocalRelationshipProvider.Add(Key, foreignKey);
            foreignKeyReturnRelationshipProvider.Add(foreignKey, Key);
        }

        public void Remove(Key foreignKey, IKVForeignKeyRelationshipProvider foreignKeyReturnRelationshipProvider)
        {
            KvLocalRelationshipProvider.Remove(Key, foreignKey);
            foreignKeyReturnRelationshipProvider.Remove(foreignKey, Key);
        }

        public void Add(KeyWithRelationship keyWithRelationship)
        {
            Add(keyWithRelationship.Key, keyWithRelationship.KvLocalRelationshipProvider);
        }

        public void Remove(KeyWithRelationship keyWithRelationship)
        {
            Remove(keyWithRelationship.Key, keyWithRelationship.KvLocalRelationshipProvider);
        }

        public IEnumerable<KeyWithRelationship> GetRelationships()
        {
            return KvLocalRelationshipProvider.GetRelationships(Key);
        }

        public IEnumerable<Key> GetReferences()
        {
            return KvLocalRelationshipProvider.GetKeys(Key);
        }

        public void Add<T>(string foreignKey, IKVStore store, IStoreSchema schema)
        {
            IObjectTypeSchema thisObjectSchema = schema.GetObjectSchema<T>();

            var relationships = thisObjectSchema.BuildKeyRelationships(store, Key);

            foreach (var relationship in relationships)
            {
                relationship.Add(this);
            }
        }

        public void Remove<T>(IKey foreignKey, IKVStore store, IStoreSchema schema)
        {
            IObjectTypeSchema thisObjectSchema = schema.GetObjectSchema<T>();

            var relationships = thisObjectSchema.BuildKeyRelationships(store, Key);

            foreach (var relationship in relationships)
            {
                relationship.Remove(this);
            }
        }
    }
}

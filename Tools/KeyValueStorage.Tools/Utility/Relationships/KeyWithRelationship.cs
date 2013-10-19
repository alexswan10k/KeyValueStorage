using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Schema;
using System.Linq;

namespace KeyValueStorage.Tools.Utility.Relationships
{
    public class KeyWithRelationship
    {
        private readonly IRelationalKey _key;
        private readonly IKVForeignKeyRelationshipProvider _kvLocalRelationshipProvider;

        public KeyWithRelationship(IRelationalKey key, IKVForeignKeyRelationshipProvider kvLocalRelationshipProvider)
        {
            _key = key;
            _kvLocalRelationshipProvider = kvLocalRelationshipProvider;
        }

        public IRelationalKey Key
        {
            get { return _key; }
        }

        public IKVForeignKeyRelationshipProvider KvLocalRelationshipProvider
        {
            get { return _kvLocalRelationshipProvider; }
        }

        public void Add(IRelationalKey foreignKey, IKVForeignKeyRelationshipProvider foreignKeyReturnRelationshipProvider)
        {
            KvLocalRelationshipProvider.Add(Key, foreignKey);
            foreignKeyReturnRelationshipProvider.Add(foreignKey, Key);
        }

        public void Remove(IRelationalKey foreignKey, IKVForeignKeyRelationshipProvider foreignKeyReturnRelationshipProvider)
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

        public IEnumerable<IRelationalKey> GetReferences()
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

        public void Remove<T>(IRelationalKey foreignKey, IKVStore store, IStoreSchema schema)
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

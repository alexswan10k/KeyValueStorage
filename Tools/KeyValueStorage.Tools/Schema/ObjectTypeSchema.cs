using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Schema
{
    public class ObjectTypeSchema : IObjectTypeSchema
    {
        private readonly Type _objectType;
        private readonly string _conceptualTableKeyPrefix;
        private readonly IDictionary<Type, string> _foreignTypeAliases;

        public ObjectTypeSchema(Type objectType, IDictionary<Type, string> foreignTypeWithAliases = null, string conceptualTableKeyPrefix = null)
        {
            _objectType = objectType;
            _conceptualTableKeyPrefix = conceptualTableKeyPrefix ?? objectType.Name + (objectType.Name.EndsWith("s")?"" : "s");//pluralize
            _foreignTypeAliases = foreignTypeWithAliases ?? new Dictionary<Type, string>();
        }

        public Type ObjectType
        {
            get { return _objectType; }
        }

        public IEnumerable<KeyWithRelationship> BuildKeyRelationships(IKVStore store, IRelationalKey key)
        {
            return _foreignTypeAliases.Select(
                s => new KeyWithRelationship(key, new KVForeignKeyStoreRelationshipProvider(store, s.Value)));
        }

        public KeyWithRelationship GetRelationshipFor<T>(IKVStore store, IRelationalKey key)
        {
            string relationshipSuffix;
            if(_foreignTypeAliases.TryGetValue(typeof (T), out relationshipSuffix))
                return new KeyWithRelationship(key, new KVForeignKeyStoreRelationshipProvider(store, relationshipSuffix));

            return null;
        }

        public string GetIncerementorKey()
        {
            //todo : replace with customisable logic
            return string.Format("{0}:i", _conceptualTableKeyPrefix);
        }

        public IRelationalKey GenerateKey(ulong sequenceValue)
        {
            //todo : replace with customisable logic
            return new RelationalKey(string.Format("{0}:{1}", _conceptualTableKeyPrefix, sequenceValue.ToString()));
        }

        public void AddRelationship<T>(string conceptualTableAlias = null)
        {
            Add(typeof (T), conceptualTableAlias);
        }

        public void Add(Type type, string conceptualTableAlias = null)
        {
            if (conceptualTableAlias == null)
                conceptualTableAlias = type.Name;

            _foreignTypeAliases.Add(type, conceptualTableAlias);
        }

        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator()
        {
            return _foreignTypeAliases.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _foreignTypeAliases.GetEnumerator();
        }
    }
}
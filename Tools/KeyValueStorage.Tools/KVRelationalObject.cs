using System;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Schema;
using KeyValueStorage.Tools.Utility.Relationships;
using System.Linq;

namespace KeyValueStorage.Tools
{
    public abstract class KVRelationalObject
    {
        protected KVRelationalObject(IRelationalKey key)
        {
            Key = key;
        }

        public IRelationalKey Key { get; internal set; }
    }

    public class KVRelationalObject<T> : KVRelationalObject
    {
        private readonly IStoreSchema _schema;
        private readonly IKVStore _store;
        private T _value;

        public T Value 
        { 
            get 
            {
                if (_value == null)
                    if (!string.IsNullOrEmpty(Key.Value))
                        _value = _store.Get<T>(Key.Value);

                return _value; 
            } 
            set
            {
                _value = value;
            } 
        }

        /// <summary>
        /// Use factory
        /// </summary>
        internal KVRelationalObject(IRelationalKey key, IStoreSchema schema, IKVStore store)
            :base(key)
        {
            _schema = schema;
            _store = store;
        }

        /// <summary>
        /// Use factory
        /// </summary>
        internal KVRelationalObject(IRelationalKey key, IStoreSchema schema, IKVStore store, T existingObject)
            :this(key, schema, store)
        {
            Value = existingObject;
        }

        private KeyWithRelationship GetRelationship<U>()
        {
            if (string.IsNullOrEmpty(Key.Value))
                throw new InvalidOperationException("Object must have a valid key before relationships can be defined. Please save the object first.");

            return _schema.GetObjectSchema<T>().GetRelationshipFor<U>(_store, Key);
        }

        public void AddRelationship<U>(KVRelationalObject<U> relationalObject)
        {
            GetRelationship<U>().Add<T>(relationalObject.Key.Value, _store, _schema);
        }

        public void RemoveRelationship<U>(KVRelationalObject<U> relationalObject)
        {
            GetRelationship<U>().Remove<T>(relationalObject.Key, _store, _schema);
        }

        public IEnumerable<IRelationalKey> GetForeignKeys<U>()
        {
            return GetRelationship<U>().GetReferences();
        }

        public IEnumerable<KVRelationalObject<U>> Get<U>() where U : new()
        {
            return GetForeignKeys<U>()
                .Select(foreignKey => new KVRelationalObject<U>(
                    foreignKey, 
                    _schema, 
                    _store, 
                    _store.Get<U>(foreignKey.Value)));
        }
    }
}

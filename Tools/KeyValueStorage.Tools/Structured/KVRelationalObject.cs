using System;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Structured.Schema;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Structured
{
    public abstract class KVRelationalObject
    {
        protected KVRelationalObject(Key key)
        {
            Key = key;
        }

        public Key Key { get; internal set; }
    }

    public class KVRelationalObject<T> : KVRelationalObject
    {
        private readonly IStoreSchema _schema;
        private readonly IKVStore _store;
        private T _value;
        private bool _isLoaded;

        public T Value 
        { 
            get 
            {
                if (Key != null && !_isLoaded && _value == null)
                {
                    _value = _store.Get<T>(Key.Value);
                    _isLoaded = true;
                }

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
        internal KVRelationalObject(Key key, IStoreSchema schema, IKVStore store)
            :base(key)
        {
            _schema = schema;
            _store = store;
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

        public IEnumerable<Key> GetForeignKeys<U>()
        {
            return GetRelationship<U>().GetReferences();
        }

        public IEnumerable<KVRelationalObject<U>> Get<U>() where U : new()
        {
            return GetForeignKeys<U>()
                .Select(foreignKey => new KVRelationalObject<U>(
                    foreignKey, 
                    _schema, 
                    _store));
        }
    }
}

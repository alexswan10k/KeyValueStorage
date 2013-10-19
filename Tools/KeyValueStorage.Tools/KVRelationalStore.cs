using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Schema;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools
{
    public class KVRelationalStore
    {
        private readonly IKVStore _store;
        private readonly IStoreSchema _schema;

        public KVRelationalStore(IKVStore store, IStoreSchema schema)
        {
            _store = store;
            _schema = schema;
        }

        public KVRelationalObject<T> Build<T>() where T : new()
        {
            return new KVRelationalObject<T>(null,_schema, _store);
        }

        public KVRelationalObject<T> Build<T>(T existingObject)
        {
            return new KVRelationalObject<T>(null, _schema, _store, existingObject);
        }

        public KVRelationalObject<T> Get<T>(IRelationalKey key)
        {
            var value = _store.Get<T>(key.Value);

            return new KVRelationalObject<T>(key, _schema, _store);
        }

        public void Set<T>(KVRelationalObject<T> obj)
        {
            if (string.IsNullOrEmpty(obj.Key.Value))
                obj.Key = _schema.GetObjectSchema<T>().GenerateKey(_store.GetNextSequenceValue(_schema.GetObjectSchema<T>().GetIncerementorKey()));
            else
            {
                _store.Set(obj.Key.Value, obj.Value);
            }
        }

        public void Remove<T>(IRelationalKey key)
        {
            //find all other references
            foreach(var rel in _schema.BuildKeyRelationships<T>(_store, key))
            {
                rel.Remove<T>(key, _store, _schema);
            }

            _store.Delete(key.Value);
        }

        public void Remove<T>(KVRelationalObject<T> value)
        {
            Remove<T>(value.Key);
            value.Key = null;
        }
    }
}

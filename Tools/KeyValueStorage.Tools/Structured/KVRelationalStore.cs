using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Structured.Schema;
using KeyValueStorage.Tools.Utility.Relationships;

namespace KeyValueStorage.Tools.Structured
{
	public interface IKVRelationalStore
	{
		IKVStore Store { get; }
		IStoreSchema Schema { get; }
		KVRelationalObject<T> New<T>() where T : new();
		KVRelationalObject<T> New<T>(T objetToWrap) where T : new();
		KVRelationalObject<T> Get<T>(Key key);
		KVRelationalObject<T> Get<T>(decimal id);
		void Save<T>(KVRelationalObject<T> obj);
		void Remove<T>(Key key);
		void Remove<T>(KVRelationalObject<T> value);
	}

	public class KVRelationalStore : IKVRelationalStore
	{
        private readonly IKVStore _store;
        private readonly IStoreSchema _schema;

        public IKVStore Store
        {
            get { return _store; }
        }

        public IStoreSchema Schema
        {
            get { return _schema; }
        }

        public KVRelationalStore(IKVStore store, IStoreSchema schema)
        {
            _store = store;
            _schema = schema;
        }

        public KVRelationalObject<T> New<T>() where T : new()
        {
            return new KVRelationalObject<T>(GenerateKey<T>(), Schema, Store);
        }

        public KVRelationalObject<T> New<T>(T objetToWrap) where T : new()
        {
            var relObject = new KVRelationalObject<T>(GenerateKey<T>(), Schema, Store);
            relObject.Value = objetToWrap;
            return relObject;
        }

        public KVRelationalObject<T> Get<T>(Key key)
        {
            var value = Store.Get<T>(key.Value);

            return new KVRelationalObject<T>(key, Schema, Store);
        }

		public KVRelationalObject<T> Get<T>(decimal id)
		{
			var key = Schema.GetObjectSchema<T>().ConceptualTableKeyPrefix + ":" + id;
			return new KVRelationalObject<T>(key,Schema, Store);
		}

        public void Save<T>(KVRelationalObject<T> obj)
        {
            if (string.IsNullOrEmpty(obj.Key.Value))
                obj.Key = GenerateKey<T>();
            else
            {
                Store.Set(obj.Key.Value, obj.Value);
            }
        }

        public void Remove<T>(Key key)
        {
            //find all other references
            foreach(var rel in Schema.BuildKeyRelationships<T>(Store, key))
            {
                rel.Remove<T>(key, Store, Schema);
            }

            Store.Delete(key.Value);
        }

        public void Remove<T>(KVRelationalObject<T> value)
        {
            Remove<T>(value.Key);
            value.Key = null;
        }

        private Key GenerateKey<T>()
        {
            return Schema.GetObjectSchema<T>().GenerateKey(Store);
        }
    }
}

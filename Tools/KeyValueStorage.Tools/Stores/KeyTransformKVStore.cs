using System;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Strings;

namespace KeyValueStorage.Tools.Stores
{
	public class KeyTransformKVStore : IKVStore
	{
		private readonly IKVStore _store;
	    private readonly IStringTransformer _transformer;

	    public KeyTransformKVStore(IKVStore underlyingStore, IStringTransformer transformer = null)
		{
		    _store = underlyingStore;
		    _transformer = transformer ?? new NullStringTransformer();
		}

	    public void Dispose()
		{
			_store.Dispose();
		}

		public IStoreProvider StoreProvider { get { return _store.StoreProvider; } }
		public ITextSerializer Serializer { get {return _store.Serializer;}}

		public T Get<T>(Key key)
		{
			return _store.Get<T>(_GetTransformedKey<T>(key));
		}

		public void Set<T>(Key key, T value)
		{
			_store.Set(_GetTransformedKey<T>(key), value);
		}

		public void Delete(Key key)
		{
			_store.Delete(_GetTransformedKey(key));
		}

		public T Get<T>(Key key, out ulong cas)
		{
			return _store.Get<T>(_GetTransformedKey<T>(key), out cas);
		}

		public void Set<T>(Key key, T value, ulong cas)
		{
			_store.Set(_GetTransformedKey<T>(key), value, cas);
		}

		public void Set<T>(Key key, T value, DateTime expires)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expires);
		}

		public void Set<T>(Key key, T value, TimeSpan expiresIn)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expiresIn);
		}

		public void Set<T>(Key key, T value, ulong CAS, DateTime expires)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expires);
		}

		public void Set<T>(Key key, T value, ulong CAS, TimeSpan expiresIn)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expiresIn);
		}

		public bool Exists(string key)
		{
			return _store.Exists(_GetTransformedKey(key));
		}

		public DateTime? ExpiresOn(string key)
		{
			return _store.ExpiresOn(_GetTransformedKey(key));
		}

		public IEnumerable<T> GetStartingWith<T>(Key key)
		{
			return _store.GetStartingWith<T>(_GetTransformedKey<T>(key));
		}

		public IEnumerable<Key> GetAllKeys()
		{
			return _store.GetAllKeys();
		}

		public IEnumerable<Key> GetKeysStartingWith(Key key)
		{
			return _store.GetKeysStartingWith(_GetTransformedKey(key));
		}

		public int CountStartingWith(Key key)
		{
			return _store.CountStartingWith(_GetTransformedKey(key));
		}

		public int CountAll()
		{
			return _store.CountAll();
		}

		public ulong GetNextSequenceValue(Key key)
		{
			return _store.GetNextSequenceValue(_GetTransformedKey(key));
		}

		public ulong GetNextSequenceValue(Key key, int increment)
		{
			return _store.GetNextSequenceValue(_GetTransformedKey(key), increment);
		}

		public IEnumerable<T> GetCollection<T>(Key key)
		{
			return _store.GetCollection<T>(_GetTransformedKey<T>(key));
		}

		public IEnumerable<T> GetCollection<T>(Key key, out ulong cas)
		{
			return _store.GetCollection<T>(_GetTransformedKey<T>(key), out cas);
		}

		public void SetCollection<T>(Key key, IEnumerable<T> values)
		{
			_store.SetCollection(_GetTransformedKey<T>(key), values);
		}

		public void SetCollection<T>(Key key, IEnumerable<T> values, ulong cas)
		{
			_store.SetCollection(_GetTransformedKey<T>(key), values,cas);
		}

		public void AppendToCollection<T>(Key key, T value)
		{
			_store.AppendToCollection(_GetTransformedKey<T>(key), value);
		}

	    public void RemoveFromCollection<T>(Key key, T value)
	    {
	        _store.RemoveFromCollection(key, value);
	    }

	    protected virtual Key _GetTransformedKey(Key key)
		{
			return _transformer.Transform(key);
		}

		protected virtual Key _GetTransformedKey<T>(Key key)
		{
			return _GetTransformedKey(key);
		}
	}
}
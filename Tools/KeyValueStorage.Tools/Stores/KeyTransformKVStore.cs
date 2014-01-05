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

		public T Get<T>(string key)
		{
			return _store.Get<T>(_GetTransformedKey<T>(key));
		}

		public void Set<T>(string key, T value)
		{
			_store.Set(_GetTransformedKey<T>(key), value);
		}

		public void Delete(string key)
		{
			_store.Delete(_GetTransformedKey(key));
		}

		public T Get<T>(string key, out ulong cas)
		{
			return _store.Get<T>(_GetTransformedKey<T>(key), out cas);
		}

		public void Set<T>(string key, T value, ulong cas)
		{
			_store.Set(_GetTransformedKey<T>(key), value, cas);
		}

		public void Set<T>(string key, T value, DateTime expires)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expires);
		}

		public void Set<T>(string key, T value, TimeSpan expiresIn)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expiresIn);
		}

		public void Set<T>(string key, T value, ulong CAS, DateTime expires)
		{
			_store.Set(_GetTransformedKey<T>(key), value, expires);
		}

		public void Set<T>(string key, T value, ulong CAS, TimeSpan expiresIn)
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

		public IEnumerable<T> GetStartingWith<T>(string key)
		{
			return _store.GetStartingWith<T>(_GetTransformedKey<T>(key));
		}

		public IEnumerable<string> GetAllKeys()
		{
			return _store.GetAllKeys();
		}

		public IEnumerable<string> GetKeysStartingWith(string key)
		{
			return _store.GetKeysStartingWith(_GetTransformedKey(key));
		}

		public int CountStartingWith(string key)
		{
			return _store.CountStartingWith(_GetTransformedKey(key));
		}

		public int CountAll()
		{
			return _store.CountAll();
		}

		public ulong GetNextSequenceValue(string key)
		{
			return _store.GetNextSequenceValue(_GetTransformedKey(key));
		}

		public ulong GetNextSequenceValue(string key, int increment)
		{
			return _store.GetNextSequenceValue(_GetTransformedKey(key), increment);
		}

		public IEnumerable<T> GetCollection<T>(string key)
		{
			return _store.GetCollection<T>(_GetTransformedKey<T>(key));
		}

		public IEnumerable<T> GetCollection<T>(string key, out ulong cas)
		{
			return _store.GetCollection<T>(_GetTransformedKey<T>(key), out cas);
		}

		public void SetCollection<T>(string key, IEnumerable<T> values)
		{
			_store.SetCollection(_GetTransformedKey<T>(key), values);
		}

		public void SetCollection<T>(string key, IEnumerable<T> values, ulong cas)
		{
			_store.SetCollection(_GetTransformedKey<T>(key), values,cas);
		}

		public void AppendToCollection<T>(string key, T value)
		{
			_store.AppendToCollection(_GetTransformedKey<T>(key), value);
		}

	    public void RemoveFromCollection<T>(string key, T value)
	    {
	        _store.RemoveFromCollection(key, value);
	    }

	    protected virtual string _GetTransformedKey(string key)
		{
			return _transformer.Transform(key);
		}

		protected virtual string _GetTransformedKey<T>(string key)
		{
			return _GetTransformedKey(key);
		}
	}
}
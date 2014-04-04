using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility.Logging;

namespace KeyValueStorage
{
	public class LoggingKVStore : IKVStore
	{
		private readonly IKVLogger _logger;
		private readonly IKVStore _store;

		public LoggingKVStore(IKVStore underlyingStore, IKVLogger logger)
		{
			_store = underlyingStore;
			_logger = logger;
		}

		#region IKeyValueStore

		public IStoreProvider StoreProvider { get { return _store.StoreProvider; } }
		public ITextSerializer Serializer { get { return _store.Serializer; } }

		public T Get<T>(Key key)
		{
			_logger.LogStoreCall("Get", key);
			try
			{
				return _store.Get<T>(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public void Set<T>(Key key, T value)
		{
			_logger.LogStoreCall("Set", key, value);
			try
			{
				_store.Set(key, value);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void Delete(Key key)
		{
			_logger.LogStoreCall("Delete", key);
			try
			{
				_store.Delete(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public T Get<T>(Key key, out ulong cas)
		{
			_logger.LogStoreCall("Get", key);
			try
			{
				var value = _store.Get<T>(key, out cas);
				_LogCasOut(cas);
				return value;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public void Set<T>(Key key, T value, ulong cas)
		{
			_logger.LogStoreCall("Set", key, value, cas);
			try
			{
				_store.Set(key, value, cas);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void Set<T>(Key key, T value, DateTime expires)
		{
			_logger.LogStoreCall("Set", key, value, null, expires);
			try
			{
				_store.Set(key, value, expires);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void Set<T>(Key key, T value, TimeSpan expiresIn)
		{
			_logger.LogStoreCall("Set", key, value, null, expiresIn);
			try
			{
				_store.Set(key, value, expiresIn);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void Set<T>(Key key, T value, ulong cas, DateTime expires)
		{
			_logger.LogStoreCall("Set", key, value, cas, expires);
			try
			{
				_store.Set(key, value, cas, expires);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void Set<T>(Key key, T value, ulong cas, TimeSpan expiresIn)
		{
			_logger.LogStoreCall("Set", key, value, cas, expiresIn);
			try
			{
				_store.Set(key, value, cas, expiresIn);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public bool Exists(string key)
		{
			_logger.LogStoreCall("Exists", key);
			try
			{
				return _store.Exists(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public DateTime? ExpiresOn(string key)
		{
			_logger.LogStoreCall("ExpiresOn", key);
			try
			{
				return _store.ExpiresOn(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		#region Queries

		public IEnumerable<T> GetStartingWith<T>(Key key)
		{
			_logger.LogStoreCall("GetStartingWith", key);
			try
			{
				return _store.GetStartingWith<T>(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public IEnumerable<Key> GetAllKeys()
		{
			_logger.LogStoreCall("GetAllKeys");
			try
			{
				return _store.GetAllKeys();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public IEnumerable<Key> GetKeysStartingWith(Key key)
		{
			_logger.LogStoreCall("GetKeysStartingWith", key);
			try
			{
				return _store.GetKeysStartingWith(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		#endregion

		#region Scalar Queries

		public int CountStartingWith(Key key)
		{
			_logger.LogStoreCall("CountStartingWith", key);
			try
			{
				return _store.CountStartingWith(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public int CountAll()
		{
			_logger.LogStoreCall("CountAll");
			try
			{
				return _store.CountAll();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		#endregion

		#region Sequences

		public ulong GetNextSequenceValue(Key key)
		{
			_logger.LogStoreCall("GetNextSequenceValue", key);
			try
			{
				return _store.GetNextSequenceValue(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public ulong GetNextSequenceValue(Key key, int increment)
		{
			_logger.LogStoreCall("GetNextSequenceValue", key, increment);
			try
			{
				return _store.GetNextSequenceValue(key, increment);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		#endregion

		#endregion

		#region CollectionOperations

		public IEnumerable<T> GetCollection<T>(Key key)
		{
			_logger.LogStoreCall("GetCollection", key);
			try
			{
				return _store.GetCollection<T>(key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public IEnumerable<T> GetCollection<T>(Key key, out ulong cas)
		{
			_logger.LogStoreCall("GetCollection", key);
			try
			{
				var collection = _store.GetCollection<T>(key, out cas);
				_LogCasOut(cas);
				return collection;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		public void SetCollection<T>(Key key, IEnumerable<T> values)
		{
			_logger.LogStoreCall("SetCollection", key, values);
			try
			{
				_store.SetCollection(key, values);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void SetCollection<T>(Key key, IEnumerable<T> values, ulong cas)
		{
			_logger.LogStoreCall("SetCollection", key, values, cas);
			try
			{
				_store.SetCollection(key, values, cas);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void AppendToCollection<T>(Key key, T value)
		{
			_logger.LogStoreCall("AppendToCollection", key, value);
			try
			{
				_store.AppendToCollection(key, value);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		public void RemoveFromCollection<T>(Key key, T value)
		{
			_logger.LogStoreCall("RemoveFromCollection", key, value);
			try
			{
				_store.RemoveFromCollection(key, value);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
			}
		}

		#endregion

		public void Dispose()
		{
			_logger.LogMessage("Disposing");
			try
			{
				_store.Dispose();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex);
				throw;
			}
		}

		private void _LogCasOut(ulong cas)
		{
			_logger.LogMessage("Cas out - " + cas);
		}
	}
}

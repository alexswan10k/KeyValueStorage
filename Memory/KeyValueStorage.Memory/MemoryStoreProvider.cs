using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C5;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.RetryStrategies;
using KeyValueStorage.Utility;
using KeyValueStorage.Utility.Logging;

namespace KeyValueStorage.Memory
{
    public class MemoryStoreProvider : IStoreProvider, IExportableStore
    {
        private readonly MemoryBackingStore _memoryBackingStore;

        public MemoryStoreProvider()
        {
            _memoryBackingStore = new MemoryBackingStore();
        }

        public void Dispose()
        {
            
        }

        #region
        public void Initialize()
        {
            
        }

        public string Get(string key)
        {
            return _memoryBackingStore[key].Value;
        }

        public void Set(string key, string value)
        {
            lock(_memoryBackingStore.GetLockObject())
            {
                _memoryBackingStore[key] = new MemoryStorageRow() {Value = value, Cas = GenerateCas()};
            }
        }

        public void Remove(string key)
        {
            _memoryBackingStore.Remove(key);
        }

        public string Get(string key, out ulong cas)
        {
            var row = _memoryBackingStore[key];
            cas = row.Cas;
            return row.Value;
        }

        public void Set(string key, string value, ulong cas)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                if(_memoryBackingStore[key].Cas != cas)
                    throw new CASException();

                _memoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas() };
            }
        }

        public void Set(string key, string value, DateTime expires)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                _memoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = expires};
            }
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                _memoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = DateTime.UtcNow + expiresIn };
            }
        }

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                if (_memoryBackingStore[key].Cas != cas)
                    throw new CASException();

                _memoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas() , Expiry = expires};
            }
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                if (_memoryBackingStore[key].Cas != cas)
                    throw new CASException();

                _memoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = DateTime.UtcNow + expiresIn };
            }
        }

        public bool Exists(string key)
        {
            //this is horribly inefficient, but otherwise we have to enumerate collection?
            try
            {
                var pointless = _memoryBackingStore[key];
                if(pointless != null)
                    return true;
            }
            catch
            {
                
            }
            return false;
        }

        public DateTime? ExpiresOn(string key)
        {
            return _memoryBackingStore[key].Expiry;
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                return _memoryBackingStore.Where(m => m.Key.StartsWith(key)).Select(s => s.Value.Value).ToArray();
            }
        }

        public IEnumerable<string> GetAllKeys()
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                return _memoryBackingStore.Keys.ToArray();
            }
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                return _memoryBackingStore.Where(m => m.Key.StartsWith(key)).Select(s => s.Key).ToArray();
            }
        }

        public int CountStartingWith(string key)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                return _memoryBackingStore.Count(m => m.Key.StartsWith(key));
            }
        }

        public int CountAll()
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                return _memoryBackingStore.Count();
            }
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
               var Val = _memoryBackingStore[key];

               if (Val == null)
                   Val = new MemoryStorageRow() {Value = "0"};

                ulong sequence = 0;
                ulong.TryParse(Val.Value, out sequence);

                sequence = sequence + (ulong) increment;

                Val.Value = sequence.ToString();
                Val.Cas = GenerateCas();

                _memoryBackingStore[key] = Val;
                return sequence;
            }
        }

        public void Append(string key, string value)
        {
            lock (_memoryBackingStore.GetLockObject())
            {
                var Val = _memoryBackingStore[key];

                if (Val == null)
                    Val = new MemoryStorageRow() { Value = string.Empty };

                Val.Value = Val.Value + value;
                _memoryBackingStore[key] = Val;
            }
        }

        public IRetryStrategy GetDefaultRetryStrategy()
        {
            return new NoRetryStrategy();
        }

		public IKeyLock GetKeyLock(string key, DateTime expires, IRetryStrategy retryStrategy = null, string workerId = null)
		{
			return new KVSLockWithCAS(key, expires, this, retryStrategy, workerId);
		}

	    #endregion

        private static ulong GenerateCas()
        {
            ulong cas;
            byte[] bytes = new byte[4];
            new Random().NextBytes(bytes);
            cas = BitConverter.ToUInt16(bytes, 0);
            return cas;
        }

        #region
        public IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup, IKVLogger logger = null)
        {
			return _memoryBackingStore.CreateBackup(createEmptyStoreBackup, logger);
        }

		public void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite, IKVLogger logger = null)
        {
			_memoryBackingStore.ApplyBackup(backupToApply, strategy, logger);
        }
        #endregion
    }
}

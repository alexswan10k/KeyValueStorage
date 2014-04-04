using System;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.RetryStrategies;
using KeyValueStorage.Utility;
using KeyValueStorage.Utility.Logging;

namespace KeyValueStorage.Memory
{
    public class SimpleMemoryStoreProvider : IStoreProvider, IExportableStore
    {
        private readonly SimpleMemoryBackingStore _memoryMemoryBackingStore;

        public SimpleMemoryStoreProvider()
        {
            _memoryMemoryBackingStore = new SimpleMemoryBackingStore();
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
            return _memoryMemoryBackingStore[key].Value;
        }

        public void Set(string key, string value)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                _memoryMemoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas() };
            }
        }

        public void Remove(string key)
        {
            _memoryMemoryBackingStore.Remove(key);
        }

        public string Get(string key, out ulong cas)
        {
            var row = _memoryMemoryBackingStore[key];
            cas = row.Cas;
            return row.Value;
        }

        public void Set(string key, string value, ulong cas)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                if (_memoryMemoryBackingStore[key].Cas != cas)
                    throw new CASException();

                _memoryMemoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas() };
            }
        }

        public void Set(string key, string value, DateTime expires)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                _memoryMemoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = expires };
            }
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                _memoryMemoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = DateTime.UtcNow + expiresIn };
            }
        }

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                if (_memoryMemoryBackingStore[key].Cas != cas)
                    throw new CASException();

                _memoryMemoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = expires };
            }
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                if (_memoryMemoryBackingStore[key].Cas != cas)
                    throw new CASException();

                _memoryMemoryBackingStore[key] = new MemoryStorageRow() { Value = value, Cas = GenerateCas(), Expiry = DateTime.UtcNow + expiresIn };
            }
        }

        public bool Exists(string key)
        {
            //this is horribly inefficient, but otherwise we have to enumerate collection?
            try
            {
                var pointless = _memoryMemoryBackingStore[key];
                if (pointless != null && pointless.Value != null)
                    return true;
            }
            catch
            {

            }
            return false;
        }

        public DateTime? ExpiresOn(string key)
        {
            return _memoryMemoryBackingStore[key].Expiry;
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                return _memoryMemoryBackingStore.Where(m => m.Key.StartsWith(key)).Select(s => s.Value.Value).ToArray();
            }
        }

        public IEnumerable<string> GetAllKeys()
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                return _memoryMemoryBackingStore.Keys.ToArray();
            }
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                return _memoryMemoryBackingStore.Where(m => m.Key.StartsWith(key)).Select(s => s.Key).ToArray();
            }
        }

        public int CountStartingWith(string key)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                return _memoryMemoryBackingStore.Count(m => m.Key.StartsWith(key));
            }
        }

        public int CountAll()
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                return _memoryMemoryBackingStore.Count();
            }
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                var Val = _memoryMemoryBackingStore[key];

                if (Val == null)
                    Val = new MemoryStorageRow() { Value = "0" };

                ulong sequence = 0;
                ulong.TryParse(Val.Value, out sequence);

                sequence =  sequence + (ulong)increment;

                Val.Value = sequence.ToString();
                Val.Cas = GenerateCas();

                _memoryMemoryBackingStore[key] = Val;
                return sequence;
            }
        }

        public void Append(string key, string value)
        {
            lock (_memoryMemoryBackingStore.GetLockObject())
            {
                var Val = _memoryMemoryBackingStore[key];

                if (Val == null)
                    Val = new MemoryStorageRow() { Value = string.Empty };

                Val.Value = Val.Value + value;
                _memoryMemoryBackingStore[key] = Val;
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

        internal static ulong GenerateCas()
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
            return _memoryMemoryBackingStore.CreateBackup(createEmptyStoreBackup, logger);
        }

        public void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite, IKVLogger logger = null)
        {
            _memoryMemoryBackingStore.ApplyBackup(backupToApply, strategy, logger);
        }
        #endregion
    }
}

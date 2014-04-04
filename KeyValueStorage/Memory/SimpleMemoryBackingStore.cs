using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility.Logging;

namespace KeyValueStorage.Memory
{
    public class SimpleMemoryBackingStore : IEnumerable<KeyValuePair<string, MemoryStorageRow>>, IExportableStore
    {
        private readonly IDictionary<string, MemoryStorageRow> _memoryStore = new Dictionary<string, MemoryStorageRow>();
        private object _storeLockObject = new object();

        public SimpleMemoryBackingStore()
        {

        }

        public MemoryStorageRow this[string key]
        {
            get { return _memoryStore[key]; }
            set { _memoryStore[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _memoryStore.Keys; }
        }

        public object GetLockObject()
        {
            return _storeLockObject;
        }

        public void Remove(string key)
        {
            _memoryStore.Remove(key);
        }

        public IEnumerator<KeyValuePair<string, MemoryStorageRow>> GetEnumerator()
        {
            return _memoryStore.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _memoryStore.GetEnumerator();
        }

        public IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup, IKVLogger logger = null)
        {
            lock (GetLockObject())
            {
                var storeBackup = createEmptyStoreBackup();
                storeBackup.AddRange(this.Select(s => s.Value.ToBackupRow(s.Key)));
                return storeBackup;
            }
        }

        public void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite, IKVLogger logger = null)
        {
            lock (GetLockObject())
            {
                foreach (var row in backupToApply)
                {
                    this[row.Key] = new MemoryStorageRow(row);
                }
            }
        }

        public void Dispose()
        {
            //do nothing
        }
    }
}
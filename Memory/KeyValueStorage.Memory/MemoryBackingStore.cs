using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C5;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Memory
{
    public class MemoryBackingStore : IEnumerable<C5.KeyValuePair<string, MemoryStorageRow>> , IExportableStore
    {
        private readonly TreeDictionary<string, MemoryStorageRow> _memoryStore = new TreeDictionary<string, MemoryStorageRow>();
        private object _storeLockObject = new object();

        public MemoryBackingStore()
        {

        }

        public MemoryStorageRow this[string key]
        {
            get { return _memoryStore[key]; }
            set { _memoryStore[key] = value; }
        }

        public ISorted<string> Keys
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

        public IEnumerator<C5.KeyValuePair<string, MemoryStorageRow>> GetEnumerator()
        {
            return _memoryStore.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _memoryStore.GetEnumerator();
        }

        public IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup)
        {
            lock (GetLockObject())
            {
                var storeBackup = createEmptyStoreBackup();
                storeBackup.AddRange(this.Select(s => s.Value.ToBackupRow(s.Key)));
                return storeBackup;
            }
        }

        public void ApplyBackup(IStoreBackup backupToApply)
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
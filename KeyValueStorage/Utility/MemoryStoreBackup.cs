using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KeyValueStorage.Interfaces
{
    public class MemoryStoreBackup : IStoreBackup
    {
        private ConcurrentBag<StoreBackupRow> _rows = new ConcurrentBag<StoreBackupRow>();

        public void Add(StoreBackupRow row)
        {
            _rows.Add(row);
        }

        public IEnumerator<StoreBackupRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }
    }
}
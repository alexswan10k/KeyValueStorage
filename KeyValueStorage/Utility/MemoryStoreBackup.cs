using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KeyValueStorage.Interfaces
{
    public class MemoryStoreBackup : IStoreBackup
    {
        private readonly ConcurrentBag<StoreBackupRow> _rows = new ConcurrentBag<StoreBackupRow>();

        public void Add(StoreBackupRow row)
        {
            _rows.Add(row);
        }

        public IEnumerator<StoreBackupRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public void AddRange(IEnumerable<StoreBackupRow> rows)
        {
            foreach(var row in rows)
            {
                _rows.Add(row);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }
    }
}
using System;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Memory
{
    public class MemoryStorageRow
    {
        public MemoryStorageRow()
        {
            
        }

        public MemoryStorageRow(StoreBackupRow backupRow)
        {
            Value = backupRow.Value;
            Cas = backupRow.Cas;
            Expiry = backupRow.Expiry;
            IsIndex = backupRow.IsIndex;
        }

        public string Value { get; set; }
        public ulong Cas { get; set; }
        public DateTime Expiry { get; set; }
        public bool IsIndex { get; set; }

        public StoreBackupRow ToBackupRow(string key)
        {
            return new StoreBackupRow() {Key = key, Value = Value, Cas = Cas, Expiry = Expiry, IsIndex = IsIndex};
        }
    }
}
using System;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Memory
{
    public class MemoryStorageRow
    {
        public MemoryStorageRow()
        {
            
        }

        public MemoryStorageRow(StoreRow row)
        {
            Value = row.Value;
            Expiry = row.Expiry;
        }

        public string Value { get; set; }
        public ulong Cas { get; set; }
        public DateTime? Expiry { get; set; }

        public StoreRow ToBackupRow(string key)
        {
            return new StoreRow() {Key = key, Value = Value, Expiry = Expiry};
        }
    }
}
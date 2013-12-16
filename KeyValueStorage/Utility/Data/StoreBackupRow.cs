using System;

namespace KeyValueStorage.Interfaces
{
    public class StoreBackupRow
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? Expiry { get; set; }
    }
}
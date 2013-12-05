using System.Collections.Generic;

namespace KeyValueStorage.Interfaces
{
    public interface IStoreBackup :IEnumerable<StoreBackupRow>
    {
        void Add(StoreBackupRow row);
    }
}
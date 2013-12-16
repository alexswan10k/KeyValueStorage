using System.Collections.Generic;

namespace KeyValueStorage.Interfaces
{
    public interface IStoreBackup :IEnumerable<StoreBackupRow>
    {
        void AddRange(IEnumerable<StoreBackupRow> rows);
		void Add(StoreBackupRow row);
    }
}
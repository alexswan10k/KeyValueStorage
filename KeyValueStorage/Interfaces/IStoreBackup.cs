using System.Collections.Generic;

namespace KeyValueStorage.Interfaces
{
    public interface IStoreBackup :IEnumerable<StoreRow>
    {
        void AddRange(IEnumerable<StoreRow> rows);
    }
}
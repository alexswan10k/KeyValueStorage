using System;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IExportableStore : IDisposable
    {
        IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup);
        //void CreateBackup(IStoreBackup backupToPopulate, IBackupStrategy strategy); //??
        void ApplyBackup(IStoreBackup backupToApply);
        //void ApplyBackup(IStoreBackup backupToApply, IBackupStrategy strategy); //??
    }
}

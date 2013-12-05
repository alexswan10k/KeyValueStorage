using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IExportableStore
    {
        void CreateBackup(IStoreBackup backupToPopulate);
        //void CreateBackup(IStoreBackup backupToPopulate, IBackupStrategy strategy); //??
        void ApplyBackup(IStoreBackup backupToApply);
        //void ApplyBackup(IStoreBackup backupToApply, IBackupStrategy strategy); //??
    }
}

using System;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IExportableStore : IDisposable
    {
        IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup);
        //void CreateBackup(IStoreBackup backupToPopulate, IBackupStrategy strategy); //??
		void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite);
        //void ApplyBackup(IStoreBackup backupToApply, IBackupStrategy strategy); //??
    }

	public enum RestoreStrategy
	{
		Overwrite,
		KeepExisting,
		UseLatest,
	}
}

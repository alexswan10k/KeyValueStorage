using System;
using System.Linq;
using System.Text;
using KeyValueStorage.Utility.Logging;

namespace KeyValueStorage.Interfaces
{
    public interface IExportableStore : IDisposable
    {
        IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup, IKVLogger logger = null);
        //void CreateBackup(IStoreBackup backupToPopulate, IBackupStrategy strategy); //??
		void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite, IKVLogger logger = null);
        //void ApplyBackup(IStoreBackup backupToApply, IBackupStrategy strategy); //??
    }

	public enum RestoreStrategy
	{
		Overwrite,
		KeepExisting,
		UseLatest,
	}
}

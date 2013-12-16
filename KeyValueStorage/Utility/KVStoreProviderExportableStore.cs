using System;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
	public class KVStoreProviderExportableStore :IExportableStore
	{
		private readonly IStoreProvider _provider;

		public KVStoreProviderExportableStore(IStoreProvider provider)
		{
			_provider = provider;
		}

		public void Dispose()
		{
			_provider.Dispose();
		}

		public IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup)
		{
			if (_provider is IExportableStore)
			{
				return (_provider as IExportableStore).CreateBackup(createEmptyStoreBackup);
			}
			else
			{
				var storeBackup = createEmptyStoreBackup();

				var keys = _provider.GetAllKeys();

				foreach (var key in keys)
				{
					DateTime? expiry = null;
					var value = _provider.Get(key);
					try
					{
						expiry = _provider.ExpiresOn(key);
					}
					catch (NotImplementedException)
					{

					}

					storeBackup.Add(new StoreBackupRow() { Expiry = expiry, Key = key, Value = value });
				}

				return storeBackup;
			}
		}

		public void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite)
		{
			if (_provider is IExportableStore)
			{
				(_provider as IExportableStore).ApplyBackup(backupToApply, strategy);
			}
			else
			{
				foreach (var item in backupToApply)
				{
					if (strategy == RestoreStrategy.Overwrite)
						_Set(item);

					else if (strategy == RestoreStrategy.KeepExisting)
					{
						var value = _provider.Get(item.Key);
						if (string.IsNullOrEmpty(value))
							_Set(item);
					}
					else if (strategy == RestoreStrategy.UseLatest)
					{
						DateTime? expiry = null;
						try
						{
							expiry = _provider.ExpiresOn(item.Key);
						}
						catch (NotImplementedException)
						{

						}

						if (expiry != null && item.Expiry == null)
							_Set(item);

						if (expiry != null && item.Expiry != null && item.Expiry > expiry)
							_Set(item);
					}
				}
			}
		}

		private void _Set(StoreBackupRow item)
		{
			if (item.Expiry != null)
				_provider.Set(item.Key, item.Value, item.Expiry.Value);
			else
				_provider.Set(item.Key, item.Value);
		}
	}
}
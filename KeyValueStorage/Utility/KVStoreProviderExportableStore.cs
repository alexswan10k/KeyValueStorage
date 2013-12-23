using System;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility.Logging;

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

		public IStoreBackup CreateBackup(Func<IStoreBackup> createEmptyStoreBackup, IKVLogger logger = null)
		{
			var localLogger = logger ?? new NullLogger();
			if (_provider is IExportableStore)
			{
				return (_provider as IExportableStore).CreateBackup(createEmptyStoreBackup, localLogger);
			}
			else
			{
				var storeBackup = createEmptyStoreBackup();
				var storeBackupEnumerable = _CreateStoreBackupEnumerable(createEmptyStoreBackup, localLogger);

				storeBackup.AddRange(storeBackup);
				return storeBackup;
			}
		}

		private IEnumerable<StoreRow> _CreateStoreBackupEnumerable(Func<IStoreBackup> createEmptyStoreBackup, IKVLogger logger)
		{
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
				{}

				yield return new StoreRow() { Expiry = expiry, Key = key, Value = value };
			}
		}

		public void ApplyBackup(IStoreBackup backupToApply, RestoreStrategy strategy = RestoreStrategy.Overwrite, IKVLogger logger = null)
		{
			var localLogger = logger ?? new NullLogger();
			if (_provider is IExportableStore)
			{
				(_provider as IExportableStore).ApplyBackup(backupToApply, strategy, localLogger);
			}
			else
			{
				foreach (var item in backupToApply)
				{
					if (strategy == RestoreStrategy.Overwrite)
						_Set(item, localLogger);

					else if (strategy == RestoreStrategy.KeepExisting)
					{
						var value = _provider.Get(item.Key);
						if (string.IsNullOrEmpty(value))
							_Set(item, localLogger);
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
							_Set(item, localLogger);

						if (expiry != null && item.Expiry != null && item.Expiry > expiry)
							_Set(item, localLogger);
					}
				}
			}
		}

		private void _Set(StoreRow item, IKVLogger logger)
		{
			if (item.Expiry != null)
				_provider.Set(item.Key, item.Value, item.Expiry.Value);
			else
				_provider.Set(item.Key, item.Value);

			logger.LogSet(item);
		}
	}
}
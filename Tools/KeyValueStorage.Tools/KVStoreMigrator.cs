using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility;
using KeyValueStorage.Utility.Logging;

namespace KeyValueStorage.Tools
{
	public interface IKVStoreMigrator
	{
		void Migrate(RestoreStrategy strategy = RestoreStrategy.Overwrite);
	}

	public class KVStoreMigrator : IKVStoreMigrator
	{
        private readonly IExportableStore _source;
        private readonly IExportableStore _destination;
		private readonly IKVLogger _logger;

		public KVStoreMigrator(IExportableStore source, IExportableStore destination, IKVLogger logger = null)
		{
			_source = source;
			_destination = destination;
			_logger = logger ?? new NullLogger();
		}

        public void Migrate(RestoreStrategy strategy = RestoreStrategy.Overwrite)
        {
            var backup = _source.CreateBackup(() => new LazyStoreBackup(), _logger);
            _destination.ApplyBackup(backup,strategy , _logger);
        }
	}
}

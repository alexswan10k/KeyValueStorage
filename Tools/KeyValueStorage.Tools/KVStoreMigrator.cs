using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools
{
	public class KVStoreMigrator
	{
        private readonly IExportableStore _source;
        private readonly IExportableStore _destination;

        public KVStoreMigrator(IExportableStore source, IExportableStore destination)
		{
			_source = source;
			_destination = destination;
		}

        public void Migrate()
        {
            var backup = new MemoryStoreBackup();
            _source.CreateBackup(backup);
            _destination.ApplyBackup(backup);
        }
	}
}

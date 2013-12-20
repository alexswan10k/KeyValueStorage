using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
	public class LazyStoreBackup : IStoreBackup
	{
		private IEnumerable<StoreBackupRow> _rows = Enumerable.Empty<StoreBackupRow>();

		public IEnumerator<StoreBackupRow> GetEnumerator()
		{
			return _rows.GetEnumerator();
		}

		public void AddRange(IEnumerable<StoreBackupRow> rows)
		{
			_rows = _rows.Concat(rows);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rows.GetEnumerator();
		}
	}
}
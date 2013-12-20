using System.Collections;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
	public class MemoryStoreBackup : IStoreBackup
	{
		private List<StoreBackupRow> _rows = new List<StoreBackupRow>();

		public IEnumerator<StoreBackupRow> GetEnumerator()
		{
			return _rows.GetEnumerator();
		}

		public void AddRange(IEnumerable<StoreBackupRow> rows)
		{
			_rows.AddRange(rows);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rows.GetEnumerator();
		}
	}
}
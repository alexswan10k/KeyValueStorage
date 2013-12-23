using System.Collections;
using System.Collections.Generic;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
	public class MemoryStoreBackup : IStoreBackup
	{
		private List<StoreRow> _rows = new List<StoreRow>();

		public IEnumerator<StoreRow> GetEnumerator()
		{
			return _rows.GetEnumerator();
		}

		public void AddRange(IEnumerable<StoreRow> rows)
		{
			_rows.AddRange(rows);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rows.GetEnumerator();
		}
	}
}
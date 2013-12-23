using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
	public class LazyStoreBackup : IStoreBackup
	{
		private IEnumerable<StoreRow> _rows = Enumerable.Empty<StoreRow>();

		public IEnumerator<StoreRow> GetEnumerator()
		{
			return _rows.GetEnumerator();
		}

		public void AddRange(IEnumerable<StoreRow> rows)
		{
			_rows = _rows.Concat(rows);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rows.GetEnumerator();
		}
	}
}
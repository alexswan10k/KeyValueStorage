using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools.Collections
{
	public interface IQueue<T> : IEnumerable<T>
	{
		void Enqueue(T item);
		T Dequeue();
		T Peek();
	}

	public class Queue<T> : IQueue<T>
	{
		private IKVStore _store;
		private readonly string _key;

		private string _idxStartKey;
		private string _idxEndKey;

		public Queue(IKVStore store, string key)
		{
			_store = store;
			_key = key;

			_idxStartKey = key + ":" + "iA";
			_idxEndKey = key + ":" + "iB";
		}

		private string GetKeyFromIdx( ulong idx)
		{
			return _key + ":" + idx;
		}

		public void Enqueue(T item)
		{
			var endIdx = _store.GetNextSequenceValue(_idxEndKey);
			_store.Set(GetKeyFromIdx(endIdx), item);
		}

		public T Dequeue()
		{
			var startIdx = _store.GetNextSequenceValue(_idxStartKey);
			string itemKey = GetKeyFromIdx(startIdx);
			var item = _store.Get<T>(itemKey);

			if (item == null)
				_store.GetNextSequenceValue(_idxEndKey);
			else
				_store.Delete(itemKey);

			return item;
		}

		private ulong GetSeqValue(string key)
		{
			return _store.Get<ulong>(key);
		}

		public T Peek()
		{
			var startIdx = GetSeqValue(_idxStartKey);
			return _store.Get<T>(GetKeyFromIdx(startIdx));
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new DualIndexStoreEnumerator<T>(_store, _key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal class DualIndexStoreEnumerator<T> : IEnumerator<T>
	{
		private readonly IKVStore _store;
		private readonly string _key;
		private ulong _startIdx;
		private ulong _endIdx;

		private ulong _enumeratorIdx;

		public DualIndexStoreEnumerator(IKVStore store, string key)
		{
			_store = store;
			_key = key;

			string _idxStartKey = key + ":" + "iA";
			string _idxEndKey = key + ":" + "iB";

			_startIdx = GetSeqValue(_idxStartKey);
			_endIdx = GetSeqValue(_idxEndKey);

			_enumeratorIdx = _startIdx;
		}

		private ulong GetSeqValue(string key)
		{
			return _store.Get<ulong>(key);
		}

		private string GetKeyFromIdx(ulong idx)
		{
			return _key + ":" + idx;
		}

		public void Dispose()
		{

		}

		public bool MoveNext()
		{
			if (_enumeratorIdx < _endIdx)
			{
				_enumeratorIdx++;
				return true;
			}

			return false;
		}

		public void Reset()
		{
			_enumeratorIdx = _startIdx;
		}

		public T Current { get { return _store.Get<T>(_key + ":" + _enumeratorIdx); } }

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools
{
	public class KVStoreMigrator
	{
		private readonly IKVStore _source;
		private readonly IKVStore _destination;

		public KVStoreMigrator(IKVStore source, IKVStore destination)
		{
			_source = source;
			_destination = destination;
		}

		public void CopyAll()
		{
			var allKeys = _source.GetAllKeys();
			_CopyKeys(allKeys);
		}

		/// <summary>
		/// This will copy all keys from the source to the desitnation but will delete any keys that are not in the source from the destination
		/// </summary>
		public void CopyAndMirrorAll()
		{
			var allKeysSource = _source.GetAllKeys();
			var allKeysDestination = _source.GetAllKeys();
			_RemoveKeysFromDestination(allKeysDestination.Except(allKeysSource));
		}

		private void _RemoveKeysFromDestination(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				_destination.Delete(key);
			}
		}

		public void CopyBetween(string prefix)
		{
			var keys = _source.GetKeysStartingWith(prefix);
			_CopyKeys(keys);
		}

		private void _CopyKeys(IEnumerable<string> allKeys)
		{
			foreach (var key in allKeys)
			{
				var value = _source.StoreProvider.Get(key);
				_destination.StoreProvider.Set(key, value);
			}
		}
	}
}

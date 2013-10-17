using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools
{
	/// <summary>
	/// Adds a prefix to the key for each call
	/// </summary>
	public class PrefixKVStore : KVStoreWithKeyTransform
	{
		private readonly string _prefix;

		public PrefixKVStore(string prefix, IKVStore underlyingStore)
			: base(underlyingStore)
		{
			_prefix = prefix;
		}

		protected override string _GetTransformedKey(string key)
		{
			return _prefix + base._GetTransformedKey(key);
		}
	}
}
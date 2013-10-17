using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools
{
	/// <summary>
	/// Adds a suffix to the key for each call
	/// </summary>
	public class SuffixKVStore: KVStoreWithKeyTransform
	{
		private readonly string _suffix;

		public SuffixKVStore(string suffix, IKVStore underlyingStore)
			:base(underlyingStore)
		{
			_suffix = suffix;
		}

		protected override string _GetTransformedKey(string key)
		{
			return base._GetTransformedKey(key) + _suffix;
		}
	}
}
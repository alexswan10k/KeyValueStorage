using System;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Tools.Stores
{
	public class TypesafeKVStore<T> : KeyTransformKVStore
	{
		public TypesafeKVStore(IKVStore underlyingStore)
			: base(underlyingStore)
		{
			
		}

		private static string GetTypeIdentifierSuffix()
		{
			return "." + typeof(T).Name;
		}

		protected override string _GetTransformedKey(string key)
		{
			return key + GetTypeIdentifierSuffix();
		}

		protected override string _GetTransformedKey<U>(string key)
		{
			if(typeof(U) != typeof(T))
				throw new InvalidOperationException(typeof(U).Name + "cannot be used with a typesafe kv store of " + typeof(T).Name);
			return _GetTransformedKey(key);
		}
	}
}

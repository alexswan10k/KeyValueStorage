using System;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Tools.Utility.Strings;
using KeyValueStorage.Utility;

namespace KeyValueStorage.Tools.Stores
{
	public class TypesafeKVStore<T> : KeyTransformKVStore
	{
		private readonly ITypeStringTransformer _generator;

		public ITypeStringTransformer Generator
		{
			get { return _generator; }
		}

		public TypesafeKVStore(IKVStore underlyingStore, ITypeStringTransformer generator = null)
			: base(underlyingStore)
		{
			_generator = generator ?? new TypeStringTransformer(null);
		}

		protected override Key _GetTransformedKey(Key key)
		{
			return Generator.TransformFor<T>(key);
		}

		protected override Key _GetTransformedKey<U>(Key key)
		{
			if(typeof(U) != typeof(T))
				throw new InvalidOperationException(typeof(U).Name + "cannot be used with a typesafe kv store of " + typeof(T).Name);
			return _GetTransformedKey(key);
		}
	}
}

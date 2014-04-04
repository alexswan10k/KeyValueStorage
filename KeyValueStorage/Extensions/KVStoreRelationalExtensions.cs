using System.Collections.Generic;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility;

namespace KeyValueStorage
{
	public static  class KVStoreRelationalExtensions
	{
		public static void AddRelationship<T, U>(this IKVStore store, Key keyT, Key keyU, ITypeStringTransformer transformer = null)
		{
			var tr = transformer ?? new ForeignKeyTypeStringTransformer();

			store.AppendToCollection(tr.TransformFor<T, U>(keyT), keyU);
			store.AppendToCollection(tr.TransformFor<U, T>(keyU), keyT);
		}

		public static void RemoveRelationship<T, U>(this IKVStore store, Key keyT, Key keyU, ITypeStringTransformer transformer = null)
		{
			var tr = transformer ?? new ForeignKeyTypeStringTransformer();

			store.RemoveFromCollection(tr.TransformFor<T, U>(keyT), keyU);
			store.RemoveFromCollection(tr.TransformFor<U, T>(keyU), keyT);
		}

		public static IEnumerable<string> GetRelatedKeysFor<T, U>(this IKVStore store, Key keyT, ITypeStringTransformer transformer = null)
		{
			var tr = transformer ?? new ForeignKeyTypeStringTransformer();
			return store.GetCollection<string>(transformer.TransformFor<T, U>(keyT));
		}
	}
}

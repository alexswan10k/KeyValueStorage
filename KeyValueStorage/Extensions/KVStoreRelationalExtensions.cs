using System.Collections.Generic;
using System.Linq;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility;

namespace KeyValueStorage
{
	public static  class KVStoreRelationalExtensions
	{
        /// <summary>
        /// Add a relationship between two objects. This is handled as a many to many relationship and will persist in both directions. 
        /// You can retrieve this from either side T or U using the GetRelatedKeysFor method
        /// </summary>
        /// <typeparam name="T">The type of the sorce object</typeparam>
        /// <typeparam name="U">The type of the related object</typeparam>
        /// <param name="store">The underlying store</param>
        /// <param name="keyT">The key of the source object</param>
        /// <param name="keyU">The key of the related object</param>
        /// <param name="transformer"></param>
		public static void AddRelationship<T, U>(this IKVStore store, Key keyT, Key keyU, ITypeStringTransformer transformer = null)
		{
			var tr = transformer ?? new ForeignKeyTypeStringTransformer();

			store.AppendToCollection(tr.TransformFor<T, U>(keyT), keyU.Value);
            store.AppendToCollection(tr.TransformFor<U, T>(keyU), keyT.Value);
		}

        /// <summary>
        /// Remove a relationship between two objects. This will remove the keys for each side of the relationship.
        /// </summary>
        /// <typeparam name="T">The type of the sorce object</typeparam>
        /// <typeparam name="U">The type of the related object</typeparam>
        /// <param name="store">The underlying store</param>
        /// <param name="keyT">The key of the source object</param>
        /// <param name="keyU">The key of the related object</param>
        /// <param name="transformer"></param>
		public static void RemoveRelationship<T, U>(this IKVStore store, Key keyT, Key keyU, ITypeStringTransformer transformer = null)
		{
			var tr = transformer ?? new ForeignKeyTypeStringTransformer();

            store.RemoveFromCollection(tr.TransformFor<T, U>(keyT), keyU.Value);
            store.RemoveFromCollection(tr.TransformFor<U, T>(keyU), keyT.Value);
		}

        /// <summary>
        /// Clears all relationships in both directtions between two types of objects connected to the source key.
        /// </summary>
        /// <typeparam name="T">The type of the sorce object</typeparam>
        /// <typeparam name="U">The type of the related object</typeparam>
        /// <param name="store">The underlying store</param>
        /// <param name="keyT">The key of the source object</param>
        /// <param name="transformer"></param>
	    public static void ClearRelationships<T, U>(this IKVStore store, Key keyT,
	        ITypeStringTransformer transformer = null)
	    {
            var tr = transformer ?? new ForeignKeyTypeStringTransformer();

            var keys = store.GetRelatedKeysFor<T, U>(keyT, tr);

	        foreach (var keyU in keys)
	        {
                store.RemoveRelationship<T, U>(keyT, keyU, tr);
	        }
	    }

        /// <summary>
        /// Gets all the related keys to objects of type U connected to keyT. This can be used in either direction
        /// </summary>
        /// <typeparam name="T">The type of the sorce object</typeparam>
        /// <typeparam name="U">The type of the related object</typeparam>
        /// <param name="store">The underlying store</param>
        /// <param name="keyT">The key of the source object</param>
        /// <param name="transformer"></param>
        /// <returns></returns>
		public static IEnumerable<string> GetRelatedKeysFor<T, U>(this IKVStore store, Key keyT, ITypeStringTransformer transformer = null)
		{
			var tr = transformer ?? new ForeignKeyTypeStringTransformer();
            return store.GetCollection<string>(tr.TransformFor<T, U>(keyT)).Distinct();
		}

        /// <summary>
        /// Retrieves all objects of type U and their retrospective keys whence given the key for type T
        /// </summary>
        /// <typeparam name="T">The type of the sorce object</typeparam>
        /// <typeparam name="U">The type of the related object</typeparam>
        /// <param name="store">The underlying store</param>
        /// <param name="keyT">The key of the source object</param>
        /// <param name="transformer"></param>
        /// <returns></returns>
	    public static IEnumerable<KeyValuePair<string, U>> GetRelatedFor<T, U>(this IKVStore store, Key keyT,
	        ITypeStringTransformer transformer = null)
	    {
            var tr = transformer ?? new ForeignKeyTypeStringTransformer();

	        var keys = store.GetRelatedKeysFor<T, U>(keyT);
	        return keys.Select(key => new KeyValuePair<string, U>(key, store.Get<U>(key)));
	    }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface ISimpleKVStore<TStoreProvider> : IDisposable where TStoreProvider : ISimpleStoreProvider
    {
        /// <summary>
        /// Gets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Get<T>(Key key);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Set<T>(Key key, T value);

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Delete(Key key);

        bool Exists(string key);
        ITextSerializer Serializer { get; }
        TStoreProvider StoreProvider { get; }
    }

    public interface ICasExpKVStore
    {
        /// <summary>
        /// Gets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="cas">The cas.</param>
        /// <returns></returns>
        T Get<T>(Key key, out ulong cas);

        /// <summary>
        /// Sets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cas">The cas.</param>
        void Set<T>(Key key, T value, ulong cas);

        /// <summary>
        /// Sets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expires">The expires.</param>
        void Set<T>(Key key, T value, DateTime expires);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        void Set<T>(Key key, T value, TimeSpan expiresIn);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="CAS">The CAS.</param>
        /// <param name="expires">The expires.</param>
        void Set<T>(Key key, T value, ulong CAS, DateTime expires);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="CAS">The CAS.</param>
        /// <param name="expiresIn">The expires in.</param>
        void Set<T>(Key key, T value, ulong CAS, TimeSpan expiresIn);

        DateTime? ExpiresOn(string key);
    }

    public interface IKVStore : ISimpleKVStore<IStoreProvider>, ICasExpKVStore
    {
        #region Queries
        IEnumerable<T> GetStartingWith<T>(Key key);
        IEnumerable<Key> GetAllKeys();
        IEnumerable<Key> GetKeysStartingWith(Key key);
        #endregion

        #region Counters
        int CountStartingWith(Key key);
        int CountAll();
        #endregion

        #region Sequences
        ulong GetNextSequenceValue(Key key);
        ulong GetNextSequenceValue(Key key, int increment);
        #endregion

        #region Collections
        IEnumerable<T> GetCollection<T>(Key key);

        IEnumerable<T> GetCollection<T>(Key key, out ulong cas);

        void SetCollection<T>(Key key, IEnumerable<T> values);

        void SetCollection<T>(Key key, IEnumerable<T> values, ulong cas);

        void AppendToCollection<T>(Key key, T value);

		void RemoveFromCollection<T>(Key key, T value);
        #endregion
    }
}

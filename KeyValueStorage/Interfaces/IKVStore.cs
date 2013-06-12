using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IKVStore : IDisposable
    {
        IStoreProvider StoreProvider { get; }
        ITextSerializer Serializer { get; }

        /// <summary>
        /// Gets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Set<T>(string key, T value);
        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Delete(string key);

        /// <summary>
        /// Gets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="cas">The cas.</param>
        /// <returns></returns>
        T Get<T>(string key, out ulong cas);
        /// <summary>
        /// Sets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cas">The cas.</param>
        void Set<T>(string key, T value, ulong cas);

        /// <summary>
        /// Sets the specified key.
        /// Supported by all providers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expires">The expires.</param>
        void Set<T>(string key, T value, DateTime expires);
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        void Set<T>(string key, T value, TimeSpan expiresIn);
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="CAS">The CAS.</param>
        /// <param name="expires">The expires.</param>
        void Set<T>(string key, T value, ulong CAS, DateTime expires);
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="CAS">The CAS.</param>
        /// <param name="expiresIn">The expires in.</param>
        void Set<T>(string key, T value, ulong CAS, TimeSpan expiresIn);

        bool Exists(string key);
        DateTime? ExpiresOn(string key);

        #region Queries
        IEnumerable<T> GetStartingWith<T>(string key);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> GetKeysStartingWith(string key);
        #endregion

        #region Counters
        int CountStartingWith(string key);
        int CountAll();
        #endregion

        #region Sequences
        ulong GetNextSequenceValue(string key);
        ulong GetNextSequenceValue(string key, int increment);
        #endregion

        #region Collections
        IEnumerable<T> GetCollection<T>(string key);

        IEnumerable<T> GetCollection<T>(string key, out ulong cas);

        void SetCollection<T>(string key, IEnumerable<T> values);

        void SetCollection<T>(string key, IEnumerable<T> values, ulong cas);

        void AppendToCollection<T>(string key, T value);
        #endregion
    }
}

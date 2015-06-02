using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces.Utility;

namespace KeyValueStorage.Interfaces
{
    public interface ISimpleStoreProvider : IDisposable
    {
        void Initialize();

        /// <summary>
        /// Gets the specified key.
        /// Covered by all providers
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// Sets the specified key.
        /// Covered by all providers
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Set(string key, string value);

        /// <summary>
        /// Removes the specified key.
        /// Supported by all providers
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        bool Exists(string key);
        IRetryStrategy GetDefaultRetryStrategy();
    }

    public interface ICasExpStoreProvider : ISimpleStoreProvider
    {
        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cas">The cas.</param>
        /// <returns></returns>
        string Get(string key, out ulong cas);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cas">The cas.</param>
        void Set(string key, string value, ulong cas);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expires">The expires.</param>
        void Set(string key, string value, DateTime expires);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        void Set(string key, string value, TimeSpan expiresIn);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="CAS">The CAS.</param>
        /// <param name="expires">The expires.</param>
        void Set(string key, string value, ulong cas, DateTime expires);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="CAS">The CAS.</param>
        /// <param name="expiresIn">The expires in.</param>
        void Set(string key, string value, ulong cas, TimeSpan expiresIn);

        DateTime? ExpiresOn(string key);
    }

    public interface IStoreProvider : ISimpleStoreProvider, ICasExpStoreProvider
    {
        #region Queries
        IEnumerable<string> GetStartingWith(string key);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> GetKeysStartingWith(string key);
        #endregion

        #region Scalar Queries
        int CountStartingWith(string key);
        int CountAll();
        #endregion

        #region Sequences
        ulong GetNextSequenceValue(string key, int increment);
        #endregion

        #region CollectionBaseOperations
        void Append(string key, string value);
        #endregion

        IKeyLock GetKeyLock(string key, DateTime expires, IRetryStrategy retryStrategy = null, string workerId = null);
    }
}

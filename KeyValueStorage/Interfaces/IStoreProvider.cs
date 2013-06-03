using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IStoreProvider : IDisposable
    {
        string Get(string key);
        void Set(string key, string value);
        void Remove(string key);

        string Get(string key, out ulong cas);
        void Set(string key, string value, ulong cas);

        void Set(string key, string value, DateTime expires);
        void Set(string key, string value, TimeSpan expiresIn);
        void Set(string key, string value, ulong CAS, DateTime expires);
        void Set(string key, string value, ulong CAS, TimeSpan expiresIn);

        bool Exists(string key);
        DateTime? ExpiresOn(string key);

        #region Queries
        IEnumerable<string> GetStartingWith(string key);
        IEnumerable<string> GetContaining(string key);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> GetKeysStartingWith(string key);
        IEnumerable<string> GetKeysContaining(string key);
        #endregion

        #region Scalar Queries
        int CountStartingWith(string key);
        int CountContaining(string key);
        int CountAll();
        #endregion

        #region Sequences
        long GetNextSequenceValue(string key, int increment);
        #endregion
    }
}

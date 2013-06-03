using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IKVStore : IDisposable
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Delete(string key);

        T Get<T>(string key, out ulong cas);
        void Set<T>(string key, T value, ulong cas);

        void Set<T>(string key, T value, DateTime expires);
        void Set<T>(string key, T value, TimeSpan expiresIn);
        void Set<T>(string key, T value, ulong CAS, DateTime expires);
        void Set<T>(string key, T value, ulong CAS, TimeSpan expiresIn);

        bool Exists(string key);
        DateTime? ExpiresOn(string key);

        #region Queries
        IEnumerable<T> GetStartingWith<T>(string key);
        IEnumerable<T> GetContaining<T>(string key);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> GetKeysStartingWith(string key);
        IEnumerable<string> GetKeysContaining(string key);
        #endregion

        #region Counters
        int CountStartingWith(string key);
        int CountContaining(string key);
        int CountAll();
        #endregion

        #region Sequences
        long GetNextSequenceValue(string key);
        long GetNextSequenceValue(string key, int increment);
        #endregion
    }
}

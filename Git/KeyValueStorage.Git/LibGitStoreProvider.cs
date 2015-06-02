using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.RetryStrategies;
using KeyValueStorage.Utility;

namespace KeyValueStorage.Git.Tests
{
    public class LibGitStoreProvider : IStoreProvider
    {
        private Repo _repo;
        public KVSExpiredKeyCleaner KeyCleaner { get; protected set; }
        const string lockPrefix = "-L-";

        public LibGitStoreProvider(string path)
        {
            _repo = new Repo(path);
            KeyCleaner = new KVSExpiredKeyCleaner(this, lockPrefix + "KC", TimeSpan.FromMinutes(1));
        }

        public void Dispose()
        {
            _repo.Dispose();
        }

        public void Initialize()
        {
            //nothing to do here
        }

        public string Get(string key)
        {
            return _repo.Get(key).Content;
        }

        public void Set(string key, string value)
        {
            _repo.Save(key, value);
        }

        public void Remove(string key)
        {
            _repo.Delete(key);
        }

        public string Get(string key, out ulong cas)
        {
            var res = _repo.Get(key);
            cas = _toULong(res.CommitSha);
            return res.Content;
        }

        public void Set(string key, string value, ulong cas)
        {
            //get latest commit and work out cas number
            //hack cheat
            ulong casCheck;
            Get(key, out casCheck);

            if(casCheck != cas)
                throw new CASException();

            _repo.Save(key, value);
        }

        public void Set(string key, string value, DateTime expires)
        {
            Set(key, value);
            _SetKeyExpiry(key, expires);
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            var expires = DateTime.UtcNow + expiresIn;

            Set(key, value);
            _SetKeyExpiry(key, expires);
        }

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            Set(key, value, cas);
            _SetKeyExpiry(key, expires);
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
        {
            var expires = DateTime.UtcNow + expiresIn;

            Set(key, value, cas);
            _SetKeyExpiry(key, expires);
        }

        public bool Exists(string key)
        {
            return _repo.Get(key).Content != String.Empty;
        }

        public DateTime? ExpiresOn(string key)
        {
            if (KeyCleaner != null)
                return KeyCleaner.GetKeyExpiry(key);
            return null;
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            return IStoreProviderInternalHelpers.GetNextSequenceValueViaCAS(this, key, increment);
        }

        public void Append(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IRetryStrategy GetDefaultRetryStrategy()
        {
            return new NoRetryStrategy();
        }

        public IKeyLock GetKeyLock(string key, DateTime expires, IRetryStrategy retryStrategy = null, string workerId = null)
        {
            throw new NotImplementedException();
        }

        private ulong _toULong(string text)
        {
            if (text == null)
                return (ulong) 1;

            return (ulong) text.GetHashCode();
        }

        private void _SetKeyExpiry(string key, DateTime expires)
        {
            if (KeyCleaner == null)
                throw new InvalidOperationException("Expiry date cannot be set if no key cleaner is present");

            KeyCleaner.SetKeyExpiry(key, expires);
        }
    }
}

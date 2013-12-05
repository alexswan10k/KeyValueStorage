
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.Utility.Data;

namespace KeyValueStorage.Utility
{
    public class KVSLockWithCAS : IKeyLock
    {
        private readonly bool _retryingLock;
        private int _retryingLockBackoffTimeMs;
        public string LockKey { get; protected set; }
        public DateTime Expires { get; protected set; }
        public string WorkerId { get; protected set; }

        public IStoreProvider Provider {get;set;}
        public ITextSerializer Serializer { get; set; }

        public KVSLockWithCAS(string lockKey, 
            DateTime expires, 
            IStoreProvider provider, 
            bool retryingLock = false, 
            string workerId = null, 
            int retryingLockBackoffTimeMs = 100, 
            ITextSerializer serializer = null)
        {
            _retryingLockBackoffTimeMs = retryingLockBackoffTimeMs;
            _retryingLock = retryingLock;
            LockKey = lockKey;
            Expires = expires;
            WorkerId = workerId ?? System.Environment.MachineName;
            Provider = provider;
            Serializer = serializer ?? new ServiceStackTextSerializer();

            if(retryingLock)
                AcquireLockRecursiveRetry();
            else
                AcquireLock();
        }

        public void AcquireLockRecursiveRetry()
        {
            try
            {
                AcquireLock();
            }
            catch(LockException)
            {
                System.Threading.Thread.Sleep(_retryingLockBackoffTimeMs);
                AcquireLockRecursiveRetry();
            }
        }

        private void AcquireLock()
        {
            ulong cas;
            var lockPOCO = Get(LockKey, out cas);
            if (lockPOCO != null)
                CheckLockPocoIsMyLock(lockPOCO);

            var storeKeyLock = new StoreKeyLock() {Expiry = Expires, WorkerId = WorkerId, IsConfirmed = true};
            if (cas > 0)
            {
                Set(LockKey, storeKeyLock, cas);
            }
            else
            {
                Set(LockKey, storeKeyLock);
            }
        }

        private void CheckLockPocoIsMyLock(StoreKeyLock lockPOCO, bool isMyLock = false)
        {
            if (lockPOCO != null)
            {
                if (lockPOCO.Expiry >= DateTime.UtcNow)
                {
                    if (WorkerId != lockPOCO.WorkerId)
                        throw new LockException("This worker " + WorkerId + " has already locked key " + LockKey);
                    else if(!isMyLock)
                        throw new LockException("Cannot acquire lock for " + LockKey + " as it has already been locked by " + WorkerId);
                }
                //otherwise the lock has expired so continue
            }
            else
                throw new ArgumentNullException("Lock POCO is null");
        }

        private void Set(string key, StoreKeyLock value)
        {
            Provider.Set(key, Serializer.Serialize(value));
        }

        private void Set(string key, StoreKeyLock value, ulong cas)
        {
            Provider.Set(key, Serializer.Serialize(value), cas);
        }

        private StoreKeyLock Get(string key, out ulong cas)
        {
            return Serializer.Deserialize<StoreKeyLock>(Provider.Get(key, out cas));
        }

        public void Dispose()
        {
            Provider.Remove(LockKey);
        }
    }
}

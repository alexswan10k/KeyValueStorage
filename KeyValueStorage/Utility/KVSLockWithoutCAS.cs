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
    public class KVSLockWithoutCAS : IKeyLock
    {
        public string LockKey { get; protected set; }
        public DateTime Expires { get; protected set; }
        public string WorkerId { get; protected set; }

        public IStoreProvider Provider {get;set;}
        public ITextSerializer Serializer { get; set; }

        public KVSLockWithoutCAS(string lockKey, DateTime expires, string workerId, IStoreProvider provider, ITextSerializer serializer)
        {
            LockKey = lockKey;
            Expires = expires;
            WorkerId = workerId;
            Provider = provider;
            Serializer = serializer;

            AcquireLock();
        }

        public KVSLockWithoutCAS(string lockKey, DateTime expires, string workerId, IStoreProvider provider)
            :this(lockKey, expires, workerId, provider, new ServiceStackTextSerializer())
        {

        }

        public KVSLockWithoutCAS(string lockKey, DateTime expires, IStoreProvider provider)
            : this(lockKey, expires, System.Environment.MachineName, provider, new ServiceStackTextSerializer())
        {

        }

        private void AcquireLock()
        {
            var lockPOCO = Get(LockKey);
            if (lockPOCO != null)
                CheckLockPocoIsMyLock(lockPOCO);

            Set(LockKey, new StoreKeyLock() { Expiry = Expires, WorkerId = WorkerId });

            lockPOCO = Get(LockKey);
            CheckLockPocoIsMyLock(lockPOCO, true);

            lockPOCO.IsConfirmed = true;
            Set(LockKey, lockPOCO);

            //finally check our lock has been sucessfully put in place by the db
            lockPOCO = Get(LockKey);

            CheckLockPocoIsMyLock(lockPOCO, true);

            if (lockPOCO.IsConfirmed == false)
                throw new Exception("Poco is in an invalid state");
        }

        private void CheckLockPocoIsMyLock(StoreKeyLock lockPOCO, bool isMyLock = false)
        {
            if (lockPOCO != null)
            {
                if (lockPOCO.Expiry >= DateTime.UtcNow)
                {
                    if (WorkerId != lockPOCO.WorkerId)
                        throw new LockException("This worker " + WorkerId + " has already locked key " + LockKey);
                    else if (!isMyLock)
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

        private StoreKeyLock Get(string key)
        {
            return Serializer.Deserialize<StoreKeyLock>(Provider.Get(key));
        }

        public void Dispose()
        {
            Provider.Remove(LockKey);
        }
    }

}

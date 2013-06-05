using KeyValueStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyValueStorage.Utility
{
    public class StoreExpiryManager
    {
        public IStoreProvider Provider { get; protected set; }
            public string LockKey {get;protected set;}
        public StoreExpiryManager(IStoreProvider provider, string lockKey)
        {
            Provider = provider;
            LockKey = lockKey;
        }

        public void BeginTaskCleanupKeys()
        {
            Task.Factory.StartNew(new Action(CleanupKeys));
        }

        public void CleanupKeys()
        {
            //grab the lock

            //grab the closest date key to datetime.utcnow and work out all keys that need to expire.

            //if any have expired, remove these keys from the store
            //remove the date-key references
            //update the date key

            //do a check for old already expired keys and process these

            //release the lock
        }

        public void SetKeyExpiry(string key, DateTime expiry)
        {
            //work out the date and quantize it into a date key window

            //get the date-key collection and cas
            //add or update the expiry
            //write back the collection


            throw new NotImplementedException();
        }

    }
}

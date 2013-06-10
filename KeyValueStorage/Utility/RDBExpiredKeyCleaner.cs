using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
    public class RDBExpiredKeyCleaner : IExpiredKeyCleaner
    {
        public IStoreProvider Provider { get; protected set; }

        public RDBExpiredKeyCleaner(IRDbStoreProvider provider)
        {
            Provider = provider;
        }

        public void CleanupKeys()
        {
            throw new NotImplementedException();
        }
    }
}

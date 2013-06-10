using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Utility
{
    public class RDBExpiredKeyCleaner : IExpiredKeyCleaner
    {
        public IStoreProvider Provider { get { return _provider; } }
        protected IRDbStoreProvider _provider;

        public RDBExpiredKeyCleaner(IRDbStoreProvider provider)
        {
            _provider = provider;
        }

        public void CleanupKeys()
        {
            throw new NotImplementedException();
        }
    }
}

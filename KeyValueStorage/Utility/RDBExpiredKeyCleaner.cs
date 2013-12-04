using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;

namespace KeyValueStorage.Utility
{
    public class RdbExpiredKeyCleaner : IExpiredKeyCleaner
    {
        public IStoreProvider Provider { get { return _provider; } }
        protected IRDbStoreProvider _provider;

        public RdbExpiredKeyCleaner(IRDbStoreProvider provider)
        {
            _provider = provider;
        }

        public void CleanupKeys()
        {
            throw new NotImplementedException();
        }

	    public void SetKeyExpiry(string key, DateTime expires)
	    {
		    throw new NotImplementedException();
	    }

	    public DateTime GetKeyExpiry(string key)
	    {
		    throw new NotImplementedException();
	    }
    }
}

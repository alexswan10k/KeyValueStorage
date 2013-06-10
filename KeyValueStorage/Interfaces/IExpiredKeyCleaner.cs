using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IExpiredKeyCleaner
    {
        IStoreProvider Provider { get; }
        void CleanupKeys();
    }
}

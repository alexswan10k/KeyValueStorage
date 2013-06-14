using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces.Utility
{
    public interface IKeyLock : IDisposable
    {
        string LockKey { get; }
        DateTime Expires { get;}
        string WorkerId { get;}
    }
}

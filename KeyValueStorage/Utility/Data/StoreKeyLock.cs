using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Data
{
    internal class StoreKeyLock
    {
        internal DateTime Expiry { get; set; }
        internal string WorkerId { get; set; }
        internal bool IsConfirmed { get; set; }
    }
}

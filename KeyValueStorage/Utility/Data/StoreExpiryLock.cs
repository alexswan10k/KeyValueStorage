using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Data
{
    internal class StoreExpiryLock
    {
        internal DateTime Expiry { get; set; }
        internal string MachineName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Data
{
    internal class StoreExpiryData
    {
        internal string TargetKey { get; set; }
        internal DateTime Expires { get; set; }
    }
}

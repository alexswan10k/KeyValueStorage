using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Data
{
    internal class StoreExpiryStateData
    {
        internal ulong SequenceCurrentVal { get; set; }
        internal DateTime LastUpdated { get; set; }
        internal DateTime WindowStart { get; set; }
    }
}

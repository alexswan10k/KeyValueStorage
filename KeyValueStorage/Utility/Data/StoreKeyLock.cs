using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Data
{
    public class StoreKeyLock
    {
        public DateTime Expiry { get; set; }
        public string WorkerId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}

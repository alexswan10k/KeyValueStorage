using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM
{
    public class ContextBase : IDisposable
    {
        Factory factory { get; protected set; }

        public ContextBase()
        {

        }

        public void Dispose()
        {
            
        }
    }
}

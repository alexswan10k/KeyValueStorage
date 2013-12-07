using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeyValueStorage.Remote
{
    /// <summary>
    /// Derive your service class from this base class
    /// </summary>
    abstract class KVStoreService : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            
        }

        public bool IsReusable { get; private set; }

        //ops
    }
}

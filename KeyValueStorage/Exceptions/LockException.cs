using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Exceptions
{
    public class LockException : Exception
    {
        public LockException(string message)
            :base(message)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Exceptions
{
    public class CASException : Exception
    {
        public CASException()
        {

        }

        public CASException(string message)
            :base(message)
        {

        }
    }
}

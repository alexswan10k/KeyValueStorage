using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.Redis
{
    public class RedisStoreProvider : IStoreProvider
    {
        public string Get(string Key)
        {
            throw new NotImplementedException();
        }

        public void Set(string Key, string Value)
        {
            throw new NotImplementedException();
        }
    }
}

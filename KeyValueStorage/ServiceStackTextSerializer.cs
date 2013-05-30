using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;
using ServiceStack.Text;

namespace KeyValueStorage
{
    public class ServiceStackTextSerializer :ITextSerializer
    {
        public string Serialize<T>(T item)
        {
            return item.ToJson();
        }

        public T Deserialize<T>(string itemSerialized)
        {
            return itemSerialized.FromJson<T>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;
using ServiceStack;
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
            try
            {
                return itemSerialized.FromJson<T>();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}

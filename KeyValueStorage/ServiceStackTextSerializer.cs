using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage
{
    public class JavaScriptTextSerializer : ITextSerializer
    {
        private JavaScriptSerializer _serializer;

        public JavaScriptTextSerializer(JavaScriptTypeResolver resolver = null)
        {
            _serializer = resolver != null ? 
                new JavaScriptSerializer(resolver) :  
                new JavaScriptSerializer();
        }

        public string Serialize<T>(T item)
        {
            return _serializer.Serialize(item);
        }

        public T Deserialize<T>(string itemSerialized)
        {
            try
            {
                //if (typeof(T) == typeof(string))
                //    return (T)(object)itemSerialized;

                return _serializer.Deserialize<T>(itemSerialized);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}

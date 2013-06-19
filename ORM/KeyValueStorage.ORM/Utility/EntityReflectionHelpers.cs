using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Utility
{
    public static class EntityReflectionHelpers
    {
        public static ulong GetEntityKey(object obj)
        {
            var props = obj.GetType().GetProperties();

            var idPropInfo = props.Single(q => q.Name == "Id");
            return (ulong)Convert.ChangeType(idPropInfo.GetGetMethod().Invoke(obj, new object[] { }), typeof(ulong));
        }

        public static void SetEntityKey(object obj, ulong key)
        {
            var props = obj.GetType().GetProperties();

            var idPropInfo = props.Single(q => q.Name == "Id");
            idPropInfo.GetSetMethod().Invoke(obj, new object[] { Convert.ChangeType(key, idPropInfo.PropertyType) });
        }
    }
}

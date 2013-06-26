using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KeyValueStorage.ORM.Mapping
{
    public class EntityMap
    {
        public EntityMap(Type entityType, MethodInfo getter, MethodInfo setter, ConstructorInfo ctor, string thisKeyFieldName, MethodInfo thisKeyGetter)
        {
            EntityType = entityType;
            DBSetPropGetter = getter;
            DbSetPropSetter = setter;
            DbSetCtor = ctor;

            KeyFieldName = thisKeyFieldName;
            ThisKeyGetter = thisKeyGetter;

            //work out the key field
            RelationshipMaps = new List<RelationshipMap>();
        }

        public string TableName { get; set; }
        public Type EntityType { get; protected set; }
        public string KeyFieldName { get; set; }
        public MethodInfo ThisKeyGetter { get; protected set; }
        public MethodInfo DBSetPropGetter { get; protected set; }
        public MethodInfo DbSetPropSetter { get; protected set; }
        public ConstructorInfo DbSetCtor { get; protected set; }
        public IList<RelationshipMap> RelationshipMaps { get; set; }   
 
        public KVSDbSet GetDbSet(ContextBase context)
        {
            return (KVSDbSet)DBSetPropGetter.Invoke(context, new object[]{});
        }

        public ulong GetKey(object obj)
        {
            return (ulong)ThisKeyGetter.Invoke(obj, new object[] { });
        }
    }
}

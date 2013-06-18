using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KeyValueStorage.ORM.Mapping
{
    public class EntityMap
    {
        public EntityMap(Type entityType, MethodInfo getter, MethodInfo setter)
        {
            EntityType = entityType;
            DBSetPropGetter = getter;
            DbSetPropSetter = setter;

            //work out the key field
        }

        public string TableName { get; set; }
        public Type EntityType { get; protected set; }
        public string KeyField { get; set; }
        public MethodInfo DBSetPropGetter { get; protected set; }
        public MethodInfo DbSetPropSetter { get; protected set; }
        public IList<RelationshipMap> RelationshipMaps { get; set; }    
    }
}

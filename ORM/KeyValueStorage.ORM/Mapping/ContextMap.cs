using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Mapping
{
    public class ContextMap
    {
        public IList<EntityMap> EntityMaps { get; set; }
        public IList<RelationshipMap> RelationshipMaps { get; set; }

        public ContextMap()
        {
            EntityMaps = new List<EntityMap>();
            RelationshipMaps = new List<RelationshipMap>();
        }

        public void InitializeContext(ContextBase context)
        {

            //magic
            foreach (var map in EntityMaps)
            {
                //get the relationship maps for each entity map

                //set the items
                map.DbSetPropSetter.Invoke(context, new object[]{(KVSDbSet)map.DbSetCtor.Invoke(new object[] { map, context })});
            }
        }
    }
}

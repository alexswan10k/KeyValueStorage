using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Mapping
{
    public class RelationshipMap
    {
        public EntityMap LocalObjectMap { get; set; }
        public EntityMap TargetObjectMap { get; set; }
        public RelationshipMapType MapType { get; set; }

        public string GetForeignTableRef (string myTableRef)
        {
            if (myTableRef == LocalObjectMap.TableName)
                return TargetObjectMap.TableName;
            else if (myTableRef == TargetObjectMap.TableName)
                return LocalObjectMap.TableName;
            else
                throw new ArgumentException(myTableRef + " does not exist for this mapping");
        }
    }

    public enum RelationshipMapType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany
    }
}

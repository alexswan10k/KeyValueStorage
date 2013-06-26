using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.ORM.Tracking;

namespace KeyValueStorage.ORM.Mapping
{
    public class RelationshipMap
    {
        public const string FKCollectionSeparator = ":F:";
        public EntityMap LocalObjectMap { get; set; }
        public EntityMap TargetObjectMap { get; set; }
        public string PropertyName { get; set; }
        public bool IsManyToTarget { get; set; }

        public string FKString
        {
            get
            {
                return LocalObjectMap.TableName + FKCollectionSeparator + TargetObjectMap.TableName;
            }
        }

        public IEnumerable<ulong> GetForeignKeys(ContextBase context)
        {
            return context.Store.GetCollection<ulong>(FKString);
        }

        //public RelationshipMapType MapType { get; set; }


        //public string GetForeignTableRef (string myTableRef)
        //{
        //    if (myTableRef == LocalObjectMap.TableName)
        //        return TargetObjectMap.TableName;
        //    else if (myTableRef == TargetObjectMap.TableName)
        //        return LocalObjectMap.TableName;
        //    else
        //        throw new ArgumentException(myTableRef + " does not exist for this mapping");
        //}

    }

    //public enum RelationshipMapType
    //{
    //    OneToOne,
    //    OneToMany,
    //    ManyToOne,
    //    ManyToMany
    //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Mapping
{
    public class RelationshipMap
    {
        public string ObjectTableA { get; set; }
        public string ObjectTableB { get; set; }
        public RelationshipMapType MapType { get; set; }

        public string GetForeignTableRef (string myTableRef)
        {
            if (myTableRef == ObjectTableA)
                return ObjectTableB;
            else if (myTableRef == ObjectTableB)
                return ObjectTableA;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.ORM.Mapping;

namespace KeyValueStorage.ORM.Tracking
{
    public class RelationshipTrackingInfo
    {
        public ObjectTrackingInfo ObjectTrackingInfo { get; set; }
        public RelationshipMap RelationshipMap { get; set; }
        public RelationshipTrackingInfoPair RelationshipTrackingPair { get; set; }

        private RelationshipTrackingInfo GetForeignRelationshipTrackingInfo()
        {
            if (RelationshipTrackingPair.RelationshipTrackingInfoA == this)
                return RelationshipTrackingPair.RelationshipTrackingInfoB;
            else if (RelationshipTrackingPair.RelationshipTrackingInfoB == this)
                return RelationshipTrackingPair.RelationshipTrackingInfoA;
            else
                throw new Exception("Pair associated does not appear to hold a valid reference to this object.");
        }

        public ulong GetForeignKey()
        {
            if (RelationshipMap == null)
                return 0;

            return RelationshipMap.TargetObjectMap.GetKey(GetForeignRelationshipTrackingInfo().ObjectTrackingInfo.ObjectRef);
        }

        public RelationshipTrackingInfo(ObjectTrackingInfo thisObjectTrackingInfo, RelationshipMap map, RelationshipTrackingInfoPair pair)
        {
            ObjectTrackingInfo = thisObjectTrackingInfo;
            RelationshipMap = map;
            RelationshipTrackingPair = pair;
        }
    }
}

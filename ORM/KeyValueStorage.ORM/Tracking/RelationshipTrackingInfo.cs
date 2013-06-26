using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.ORM.Mapping;

namespace KeyValueStorage.ORM.Tracking
{
    public class RelationshipTrackingInfo
    {
        public ObjectTrackingInfo ThisObjectTrackingInfo { get; set; }
        public ObjectTrackingInfo TargetObjectTrackingInfo { get; set; }
        public RelationshipMap RelationshipMap { get; set; }
        public RelationshipState RelationshipChangeState { get; set; }

        public ulong GetForeignKey()
        {
            return RelationshipMap.TargetObjectMap.GetKey(TargetObjectTrackingInfo.ObjectRef);
        }

        public RelationshipTrackingInfo(ObjectTrackingInfo thisObjectTrackingInfo, ObjectTrackingInfo targetObjectTrackingInfo, RelationshipMap map, RelationshipState state)
        {
            ThisObjectTrackingInfo = thisObjectTrackingInfo;
            TargetObjectTrackingInfo = targetObjectTrackingInfo;
            RelationshipMap = map;
            RelationshipChangeState = state;
        }
    }

    public enum RelationshipState
    {
        Added,
        Removed,
        Unchanged
    }
}

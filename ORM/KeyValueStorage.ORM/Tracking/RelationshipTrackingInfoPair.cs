using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Tracking
{
    public class RelationshipTrackingInfoPair
    {
        public RelationshipTrackingInfo RelationshipTrackingInfoA { get; set; }
        public RelationshipTrackingInfo RelationshipTrackingInfoB { get; set; }
        public RelationshipState RelationshipChangeState { get; set; }
    }

    public enum RelationshipState
    {
        Added,
        Removed,
        Unchanged
    }
}

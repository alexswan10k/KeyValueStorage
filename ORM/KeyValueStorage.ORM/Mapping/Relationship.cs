using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Mapping
{
    public class Relationship
    {
        public ContextBase Context { get; set; }
        public KVSDbSet LocalDbSet { get; set; }
        public KVSDbSet TargetDbSet { get; set; }
        public RelationshipMap RelationshipMap { get; set; }

        //public void UpdateRelationshipObject(object obj)
        //{
        //    ObjectTrackingInfo trkInfo;
        //    if (!Context.ObjectTracker.ObjectsToTrack.TryGetValue(obj, out trkInfo))
        //        return;
        //}
    }
}

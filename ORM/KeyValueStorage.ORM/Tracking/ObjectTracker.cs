using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Tracking
{
    public class ObjectTracker
    {
        public Dictionary<object, ObjectTrackingInfo> ObjectsTracking { get; protected set; }
        public Dictionary<object, RelationshipTrackingInfo> RelationshipToTrack { get; protected set; }
        public ContextBase Context { get; protected set; }

        public ObjectTracker(ContextBase context)
        {
            ObjectsTracking = new Dictionary<object, ObjectTrackingInfo>();
            RelationshipToTrack = new Dictionary<object, RelationshipTrackingInfo>();
            Context = context;
        }

        public ObjectTrackingInfo GetTrackingInfo(object obj)
        {
            if(obj == null)
                throw new ArgumentNullException("obj");

            ObjectTrackingInfo objTrkInfo;

            if (!ObjectsTracking.TryGetValue(obj, out objTrkInfo))
            {
                var entityMap = Context.ContextMap.EntityMaps.SingleOrDefault(q => q.EntityType == obj.GetType() || q.EntityType == obj.GetType().BaseType);
               if (entityMap == null)
                   throw new Exception("Entity does not have an associated map.");

                objTrkInfo = new ObjectTrackingInfo(obj, entityMap.GetDbSet(Context), false);
                //entityMap.RelationshipMaps.Select(
                ObjectsTracking.Add(obj, objTrkInfo);
            }

            return objTrkInfo;
        }

        public bool TryAttachObject(object obj)
        {
            if (ObjectsTracking.ContainsKey(obj))
                return false;

            AttachObject(obj);
            return true;
        }

        public ObjectTrackingInfo AttachObject(object objectToTrack)
        {
            //implement same as above but kindly throw exceptions instead

            if (ObjectsTracking.ContainsKey(objectToTrack))
                throw new Exception("Object is already being tracked");

            return GetTrackingInfo(objectToTrack);
        }

        public void DetachObject(object objectToDetach)
        {
            ObjectsTracking.Remove(objectToDetach);

            //traverse relationships and perform this recursively
        }

        public IEnumerable<object> GetChanges()
        {
            return from q in ObjectsTracking
                   where q.Value.State == ObjectTrackingInfoState.Changed
                   select q.Key;
        }

        public IEnumerable<object> GetNew()
        {
            return from q in ObjectsTracking
                   where q.Value.State == ObjectTrackingInfoState.New
                   select q.Key;
        }

        public IEnumerable<object> GetDeleted()
        {
            return from q in ObjectsTracking
                   where q.Value.State == ObjectTrackingInfoState.FlaggedForDeletion
                   select q.Key;
        }

        public ObjectTrackingInfo GetObjectTrackingInfo(object obj)
        {
            ObjectTrackingInfo info;
            ObjectsTracking.TryGetValue(obj, out info);
            return info;
        }
    }
}

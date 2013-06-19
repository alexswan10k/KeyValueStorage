using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.ORM.Tracking
{
    public class ObjectTracker
    {
        public Dictionary<object, ObjectTrackingInfo> ObjectsToTrack { get; set; }

        public ObjectTracker()
        {
            ObjectsToTrack = new Dictionary<object, ObjectTrackingInfo>();
        }

        public bool TryAttachObject(object objectToTrack, ObjectTrackingInfo objectTrackingInfo)
        {
            //Implement key checking!
            if (ObjectsToTrack.Keys.Any(q => q.Equals(objectToTrack)))
            {
                //we are already tracking this object, get the object and return it
                return false;
            }

            ObjectsToTrack.Add(objectToTrack, objectTrackingInfo);
            return true;
        }

        public void AttachObject(object objectToTrack, ObjectTrackingInfo objectTrackingInfo)
        {
            //implement same as above but kindly throw exceptions instead

            if (!TryAttachObject(objectToTrack, objectTrackingInfo))
                throw new Exception("Object is already being tracked");
        }

        public void DetachObject(object objectToDetach)
        {
            ObjectsToTrack.Remove(objectToDetach);
        }

        public IEnumerable<object> GetChanges()
        {
            return from q in ObjectsToTrack
                   where q.Value.State == ObjectTrackingInfoState.Changed
                   select q.Key;
        }

        public IEnumerable<object> GetNew()
        {
            return from q in ObjectsToTrack
                   where q.Value.State == ObjectTrackingInfoState.New
                   select q.Key;
        }

        public IEnumerable<object> GetDeleted()
        {
            return from q in ObjectsToTrack
                   where q.Value.State == ObjectTrackingInfoState.FlaggedForDeletion
                   select q.Key;
        }
    }
}

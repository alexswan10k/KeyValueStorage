using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using KeyValueStorage.ORM.Utility;

namespace KeyValueStorage.ORM.Tracking
{
    public class ObjectTrackingInfo
    {
        public ulong Key
        {
            get
            {
                return EntityReflectionHelpers.GetEntityKey(ObjectRef);
            }
        }

        public KVSDbSet DbSetAssociatedWith { get; protected set; }

        public object ObjectRef{get;protected set;}
        public Dictionary<string, string> InitialValues{get; protected set;}
        public IList<RelationshipTrackingInfo> Relationships { get; protected set; }

        public ObjectTrackingInfo(object objectRef, KVSDbSet setAssociatedWith, bool isNew)
        {
            ObjectRef = objectRef;
            DbSetAssociatedWith = setAssociatedWith;
            InitialValues = objectRef.ToStringDictionaryExcludingRefs();
            if (isNew)
                State = ObjectTrackingInfoState.New;
            else
                State = ObjectTrackingInfoState.Unchanged;

            Relationships = new List<RelationshipTrackingInfo>();
        }

        public bool HasChanges()
        {
            var dictCurrent = ObjectRef.ToStringDictionaryExcludingRefs();

            if(!InitialValues.Any(q=>dictCurrent[q.Key] == q.Value))
                return false;

            return true;
        }

        public Dictionary<string, string> GetChanges()
        {   
            var dictCurrent = ObjectRef.ToStringDictionaryExcludingRefs();
            Dictionary<string, string> changeDict = new Dictionary<string,string>();

            foreach(var val in InitialValues)
            {
                if(dictCurrent[val.Key] != val.Value)
                    changeDict.Add(val.Key, val.Value);
            }
            
            return changeDict;
        }

        protected ObjectTrackingInfoState state;
        public ObjectTrackingInfoState State
        {
            get
            {
                if(state == ObjectTrackingInfoState.Unchanged)
                {
                    if(HasChanges())
                        return ObjectTrackingInfoState.Changed;
                }

                return state;
            }
            set
            {
                state = value;
            }
        }
    }

        public enum ObjectTrackingInfoState
    {
        Unchanged,
        Changed,
        New,
        FlaggedForDeletion,
        Deleted
    }
}


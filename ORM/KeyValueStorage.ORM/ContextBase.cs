using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.ORM.Mapping;
using KeyValueStorage.ORM.Tracking;
using KeyValueStorage.ORM.Utility;

namespace KeyValueStorage.ORM
{
    public abstract class ContextBase : IDisposable
    {
        static ContextBase()
        {
            ContextMaps = new ConcurrentDictionary<Type, ContextMap>();
        }

        public IKVStore Store { get; protected set; }
        public ObjectTracker ObjectTracker { get; private set; }
        public ContextMap ContextMap { get; private set; }

        public ContextBase()
        {
            Store = KVStore.Factory.Get();

            ContextMap contextMap = null;

            if (!ContextMaps.ContainsKey(this.GetType()))
                contextMap = SetupMaps(this);
            else
                contextMap = ContextMaps[this.GetType()];

            ContextMap = contextMap;
            SetupDbSets();

            ObjectTracker = new ObjectTracker();
            contextMap.InitializeContext(this);
            
        }

        public void SetupDbSets()
        {
            //foreach (var prop in this.GetType().GetProperties())
            //{
            //    if (prop.PropertyType.Name == "KVSDbSet`1" || prop.PropertyType.Name == "ICollection`1")
            //    {
            //        var targetType = prop.PropertyType.GetGenericArguments().First();
            //        var ctor = typeof(KVSDbSet<>).MakeGenericType(targetType).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).First();

                    

            //        var map = ContextMap.EntityMaps.Single(q => q.EntityType == targetType);

            //        var obj = ctor.Invoke(new object[] { map, this });

            //        prop.GetSetMethod().Invoke(this, new object[] { obj });
            //    }
            //}

            foreach (var map in this.ContextMap.EntityMaps)
            {
                map.DbSetPropSetter.Invoke(this, new object[]{map.DbSetCtor.Invoke(new object[] { map, this })});
            }
        }

        public void Dispose()
        {
            Store.Dispose();
        }

        protected static ContextMap SetupMaps(ContextBase thisItem)
        {
            if (ContextMaps == null)
                ContextMaps = new ConcurrentDictionary<Type, ContextMap>();

            ContextMap contextMap = new ContextMap();

            foreach (var prop in thisItem.GetType().GetProperties())
            {
                if (prop.PropertyType.Name == "KVSDbSet`1" || prop.PropertyType.Name == "ICollection`1")
                {
                    Type entityType = prop.PropertyType.GetGenericArguments().Last();

                    var entityMap = new EntityMap(entityType, prop.GetGetMethod(), prop.GetSetMethod(), prop.PropertyType.GetConstructor(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[]{typeof(EntityMap), typeof(ContextBase)}, new System.Reflection.ParameterModifier[]{}));
                    contextMap.EntityMaps.Add(entityMap);
                }
            }

            foreach (var entityMap in contextMap.EntityMaps)
            {
                foreach (var prop in entityMap.EntityType.GetProperties())
                {
                    if (prop.PropertyType.Name == "KVSCollection`1" || prop.PropertyType.Name == "ICollection`1")
                    {
                        Type targetEntityType = prop.PropertyType.GetGenericArguments().Last();
                        var targetEntityMap = contextMap.EntityMaps.SingleOrDefault(q => q.EntityType == targetEntityType);

                        if (targetEntityMap != null)
                        {
                            var relationshipMap = new RelationshipMap() 
                            { 
                                LocalObjectMap = entityMap, 
                                TargetObjectMap = targetEntityMap, 
                                PropertyName = prop.Name, 
                                IsManyToTarget = true 
                            };

                            contextMap.RelationshipMaps.Add(relationshipMap);
                            entityMap.RelationshipMaps.Add(relationshipMap);
                        }
                        //many relationship
                    }
                    else if (contextMap.EntityMaps.Any(q => q.EntityType == prop.PropertyType))
                    {
                        //foreign key ref (single relationship)

                        Type targetEntityType = prop.PropertyType;
                        var targetEntityMap = contextMap.EntityMaps.SingleOrDefault(q => q.EntityType == targetEntityType);

                        if (targetEntityMap != null)
                        {
                            var relationshipMap = new RelationshipMap() { LocalObjectMap = entityMap, TargetObjectMap = targetEntityMap };
                            contextMap.RelationshipMaps.Add(relationshipMap);
                            entityMap.RelationshipMaps.Add(relationshipMap);
                        }
                    }
                }

                //find relationships
            }

            ContextMaps.TryAdd(thisItem.GetType(), contextMap);

            return contextMap;
        }

        public void AttachObject(object obj)
        {
            var entityMap = ContextMap.EntityMaps.SingleOrDefault(q => q.EntityType == obj.GetType());
            if (entityMap == null)
                throw new Exception("Object type " + obj.GetType().ToString() + " is not supported by context");

            ObjectTracker.AttachObject(obj, new ObjectTrackingInfo(obj, entityMap.GetDbSet(this), false));
        }

        public void DetachObject(object obj)
        {
            ObjectTracker.DetachObject(obj);
        }

        public virtual void SaveChanges()
        {
            foreach (var objKV in ObjectTracker.ObjectsToTrack)
            {
                if (objKV.Value.State == ObjectTrackingInfoState.New 
                    || objKV.Value.State == ObjectTrackingInfoState.Changed 
                    || objKV.Value.State == ObjectTrackingInfoState.FlaggedForDeletion)
                {
                    var objTrackingInfo = objKV.Value;

                    if (objKV.Value.State == ObjectTrackingInfoState.New)
                    {
                        var seq = objTrackingInfo.DbSetAssociatedWith.GetNextSequenceValue();
                        EntityReflectionHelpers.SetEntityKey(objKV.Key, seq);
                        Store.Set(objTrackingInfo.DbSetAssociatedWith.BaseKey + KVSDbSet.CollectionPrefix + EntityReflectionHelpers.GetEntityKey(objKV.Key), objKV.Key.ToStringDictionaryExcludingRefs());
                    }
                    else if (objKV.Value.State == ObjectTrackingInfoState.Changed)
                    {
                        Store.Set(objTrackingInfo.DbSetAssociatedWith.BaseKey + KVSDbSet.CollectionPrefix + EntityReflectionHelpers.GetEntityKey(objKV.Key), objKV.Key.ToStringDictionaryExcludingRefs());
                        objKV.Value.State = ObjectTrackingInfoState.Unchanged;
                    }
                    else if (objKV.Value.State == ObjectTrackingInfoState.FlaggedForDeletion)
                    {
                        var seq = EntityReflectionHelpers.GetEntityKey(objKV.Key);
                        Store.Delete(objTrackingInfo.DbSetAssociatedWith.BaseKey + KVSDbSet.CollectionPrefix + EntityReflectionHelpers.GetEntityKey(objKV.Key));
                        objKV.Value.State = ObjectTrackingInfoState.Deleted;
                    }
                }
            }
        }

        protected static ConcurrentDictionary<Type, ContextMap> ContextMaps { get; private set; }
    }
}

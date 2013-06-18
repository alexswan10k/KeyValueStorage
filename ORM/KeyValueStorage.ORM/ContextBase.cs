using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.ORM.Mapping;

namespace KeyValueStorage.ORM
{
    public abstract class ContextBase : IDisposable
    {
        static ContextBase()
        {
            ContextMaps = new ConcurrentDictionary<Type, ContextMap>();
        }

        public IKVStore Store { get; protected set; }

        public ContextBase()
        {
            Store = KVStore.Factory.Get();

            if (!ContextMaps.ContainsKey(this.GetType()))
                SetupMaps(this);

            var map = ContextMaps[this.GetType()];
            map.InitializeContext(this);
        }

        public void Dispose()
        {
            
        }

        protected static void SetupMaps(ContextBase thisItem)
        {
            if (ContextMaps == null)
                ContextMaps = new ConcurrentDictionary<Type, ContextMap>();

            ContextMap contextMap = new ContextMap();

            foreach (var prop in thisItem.GetType().GetProperties())
            {
                if (prop.PropertyType.Name == "KVSDbSet`1" || prop.PropertyType.Name == "ICollection`1")
                {
                    Type entityType = prop.PropertyType.GetGenericArguments().Last();

                    var entityMap = new EntityMap(entityType, prop.GetGetMethod(), prop.GetSetMethod());
                    contextMap.EntityMaps.Add(entityMap);
                }
            }

            foreach (var entityMap in contextMap.EntityMaps)
            {
                foreach (var prop in entityMap.EntityType.GetProperties())
                {
                    if (prop.PropertyType.Name == "KVSCollection`1" || prop.PropertyType.Name == "ICollection`1")
                    {
                        entityMap.RelationshipMaps.Add(new RelationshipMap() { LocalObjectMap = entityMap, MapType = RelationshipMapType.ManyToMany });
                        //many relationship
                    }
                    else if (contextMap.EntityMaps.Any(q => q.EntityType == prop.PropertyType))
                    {
                        //foreign key ref (single relationship)
                    }
                }

                //find relationships
            }

            ContextMaps.TryAdd(thisItem.GetType(), contextMap);
        }

        protected static ConcurrentDictionary<Type, ContextMap> ContextMaps { get; private set; }
    }
}

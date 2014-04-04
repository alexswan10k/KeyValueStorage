using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.ORM.Mapping;
using KeyValueStorage.ORM.Tracking;
using KeyValueStorage.ORM.Utility;
using ServiceStack.Text;

namespace KeyValueStorage.ORM
{
    public abstract class KVSDbSet : IEnumerable
    {
        public string BaseKey { get; protected set; }
        public const string CollectionPrefix = ":C:";
        public const string SequenceSuffix = ":S";
        public ContextBase Context { get; set; }
        public EntityMap EntityMap { get; set; }

        public virtual IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public ulong GetNextSequenceValue()
        {
            return Context.ObjectMaterializer.Store.GetNextSequenceValue(BaseKey + SequenceSuffix);
        }

        public void Add(object obj)
        {
            throw new NotImplementedException();
        }

        public abstract object GetByIdWeak(ulong id);
    }

    public class KVSDbSet<T> : KVSDbSet, ICollection<T> where T : class
    {
        public IDictionary<ulong, T> KeyedItems { get; protected set; }
        public bool CachedIsFullList { get; protected set; }

        internal KVSDbSet(EntityMap map, ContextBase context)
        {
            KeyedItems = new Dictionary<ulong, T>();
            Context = context;
            EntityMap = map;
            BaseKey = map.EntityType.Name;
        }

        protected void LoadIfNotLoaded()
        {
            if (!CachedIsFullList)
                RefreshKeyedItems();

            CachedIsFullList = true;
        }

        protected void RefreshKeyedItems()
        {
            var keys = Context.ObjectMaterializer.Store.GetKeysStartingWith(BaseKey + CollectionPrefix);
            var collection = Context.ObjectMaterializer.GetStartingWith<T>(BaseKey + CollectionPrefix);

            if (keys.Count() != collection.Count())
                throw new InvalidProgramException("Keys returned and collection do not match");

            KeyedItems.Clear();

            for (int i = 0; i < keys.Count(); i++)
            {
                var keyIdx = ulong.Parse(keys.ElementAt(i).ToString().Split(':').Last());
                var entity = collection.ElementAt(i);
                if (EntityReflectionHelpers.GetEntityKey(entity) == 0)
                    EntityReflectionHelpers.SetEntityKey(entity, keyIdx);
                KeyedItems.Add(keyIdx, entity);
                Context.ObjectTracker.TryAttachObject(entity);
            }

            //if (CollectionRefreshed != null)
            //    CollectionRefreshed.Invoke(this, CachedCollection);
        }

        public void Add(T item)
        {
            ObjectTrackingInfo trackInfo;
            item = CreateProxy(item);
            if (!Context.ObjectTracker.ObjectsTracking.TryGetValue(item, out trackInfo))
            {
                trackInfo = Context.ObjectTracker.AttachObject(item);
                trackInfo.State = ObjectTrackingInfoState.New;
            }
            else
            {
                trackInfo.State = ObjectTrackingInfoState.New;
            }
        }

        public void Clear()
        {
            if (!CachedIsFullList)
                LoadIfNotLoaded();

            foreach (var item in KeyedItems)
            {
                Context.ObjectTracker.DetachObject(item);
            }

            KeyedItems.Clear();
        }

        public bool Contains(T item)
        {
            if (KeyedItems.Values.Contains(item))
                return true;
            else if(KeyedItems.Select( s=> s.Value.Dump()).Contains(item.Dump()))
                return true;

            RefreshKeyedItems();

            if (KeyedItems.Values.Contains(item))
                return true;
            else if (KeyedItems.Select(s => s.Value.Dump()).Contains(item.Dump()))
                return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < arrayIndex + this.Count; i++)
            {
                array[i] = this.ElementAt(i - arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                if (!CachedIsFullList)
                    LoadIfNotLoaded();

                return KeyedItems.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            ObjectTrackingInfo trackInfo = Context.ObjectTracker.GetObjectTrackingInfo(item);

            trackInfo.State = ObjectTrackingInfoState.FlaggedForDeletion;

            if (KeyedItems.ContainsKey(trackInfo.Key))
                return KeyedItems.Remove(trackInfo.Key);

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!CachedIsFullList)
                LoadIfNotLoaded();

            return KeyedItems.Values.GetEnumerator();
        }

        public T GetById(ulong id)
        {
            T item;

            if (!KeyedItems.TryGetValue(id, out item))
            {
                item = Context.ObjectMaterializer.GetObject<T>(BaseKey + CollectionPrefix + id);

                if (item != null)
                    Context.ObjectTracker.TryAttachObject(item);
            }
            return item;
        }

        public override object GetByIdWeak(ulong id)
        {
            return GetById(id);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!CachedIsFullList)
                LoadIfNotLoaded();

            return KeyedItems.Values.GetEnumerator();
        }

        public T CreateProxy(T obj)
        {
            Castle.DynamicProxy.ProxyGenerator pr = new Castle.DynamicProxy.ProxyGenerator();
            var interceptor = new AssocTypeInterceptor(this.Context);
            return (T)pr.CreateClassProxy(typeof(T), interceptor);
        }

        //public event EventHandler<IDictionary<ulong, T>> CollectionRefreshed;
        //public event EventHandler<KeyValuePair<ulong, T>> ItemAdded;
        //public event EventHandler<KeyValuePair<ulong, T>> ItemRemoved;
    }
}

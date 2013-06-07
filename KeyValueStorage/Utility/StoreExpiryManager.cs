using KeyValueStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Exceptions;

namespace KeyValueStorage.Utility
{
    public class StoreExpiryManager
    {
        public IStoreProvider Provider { get; protected set; }
            public string LockKey {get;protected set;}
            public string StoreExpirySequenceKey { get; protected set; }
            public string StoreExpiryStateDataKey { get; protected set; }
            public int LockExpiryTimeS { get; protected set; }
            public int Period_ms{ get; protected set;}
            public ITextSerializer Serializer { get; protected set; }
            public string StoreExpiryDateRowPrefix { get; protected set; }
            public TimeSpan WindowResolution { get; protected set; }

        public StoreExpiryManager(IStoreProvider provider, string lockKey)
        {
            Provider = provider;
            LockKey = lockKey;
            Period_ms = 60000;
            WindowResolution = TimeSpan.FromMilliseconds(Period_ms);
            LockExpiryTimeS = 320;
            Serializer = new ServiceStackTextSerializer();
            StoreExpiryDateRowPrefix = "-SE-";
            StoreExpirySequenceKey = "-SES";
            StoreExpiryStateDataKey = "-SESD";
        }

        public void BeginTaskCleanupKeys()
        {
            tickTimer = new System.Threading.Timer(new System.Threading.TimerCallback(o =>
            {

                if (cleanupTask == null || cleanupTask.IsCompleted || cleanupTask.IsFaulted)
                    Task.Factory.StartNew(new Action(CleanupKeys));
            }), null, 0, Period_ms);

        }

        public System.Threading.Timer tickTimer { get; protected set; }
        Task cleanupTask = null;

        public void CleanupKeys()
        {
            ulong lockCas;
            var lockVal = Serializer.Deserialize <StoreExpiryLock>(Provider.Get(LockKey, out lockCas));

            if(lockVal == null || lockVal.Expiry < DateTime.UtcNow)
            {
                //!Set a lock for this machine
                Provider.Set(LockKey, Serializer.Serialize(new StoreExpiryLock(){
                 Expiry = DateTime.UtcNow.AddSeconds(LockExpiryTimeS),
                 MachineName = System.Environment.MachineName, 
                }), lockCas);

                var stateData = GetStateData();

                //check row-1 to avoid race conditions in the unlikely event where a reference is written into window n and the sequence is moved on to n+1
                RemoveItemsFromWindow(stateData.SequenceCurrentVal-1);

                //If we can clear multiple rows, do this. This will not normally occur. Allows catching up if data has not yet been processed.
                while (RemoveItemsFromWindow(stateData.SequenceCurrentVal) == 0)
                {
                    stateData.SequenceCurrentVal = Provider.GetNextSequenceValue(StoreExpirySequenceKey, 1);
                    stateData.LastUpdated = DateTime.UtcNow;
                    stateData.WindowStart = DateTime.UtcNow;
                    Provider.Set(StoreExpiryStateDataKey, Serializer.Serialize(stateData));
                }



                    //release the lock
                    Provider.Remove(LockKey);
            }
        }

        public void SetKeyExpiry(string key, DateTime expires)
        {
            //work out the date and quantize it into a date key window

            //get the date-key collection and cas
            //add or update the expiry
            //write back the collection
            var stateData = GetStateData();
            var window = GetWindow(expires, stateData);

            Provider.Append(StoreExpiryDateRowPrefix + "-" + window, Serializer.Serialize(new StoreExpiryData(){ TargetKey = key, Expires = expires}));
        }

        protected int RemoveItemsFromWindow(ulong offsetWindow)
        {
            try
            {
                //grab the currnent sequence value
                ulong dataRowCAS;
                var dataRow = GetExpiryDataRow(offsetWindow, out dataRowCAS);
                List<StoreExpiryData> dataRowItemsToRemove = new List<StoreExpiryData>();
                foreach (var item in dataRow)
                {
                    if (item.Expires < DateTime.UtcNow)
                    {
                        dataRowItemsToRemove.Add(item);
                    }
                }

                //remove our rows
                foreach (var item in dataRowItemsToRemove)
                {
                    //If the row has somehow already been removed this will not fail.
                    Provider.Remove(item.TargetKey);
                }

                //If all hve been succesfully removed, we can update our refs.
                var itemsLeft = dataRow.Except(dataRowItemsToRemove).ToList();
                SetExpiryDataRow(offsetWindow, itemsLeft, dataRowCAS);
                return itemsLeft.Count();
            }
            catch (CASException ex)
            {
                //CAS mismatch. With locking this should not occur. Retry?
            }
            return -1;
        }

        protected ulong GetWindow(DateTime date, StoreExpiryStateData stateData)
        {
            var windowStartDate = stateData.WindowStart;

            if (date < windowStartDate)
                throw new Exception("Date is less than current date");

            ulong i = stateData.SequenceCurrentVal;

            while (date > windowStartDate)
            {
                windowStartDate = windowStartDate + WindowResolution;
                i++;
            }
            return i;
        }

        protected StoreExpiryStateData GetStateData()
        {
            StoreExpiryStateData stateData = null;
            Serializer.Deserialize<StoreExpiryStateData>(Provider.Get(StoreExpiryStateDataKey));
            if (stateData == null)
            {
                stateData = new StoreExpiryStateData()
                {
                    SequenceCurrentVal = Provider.GetNextSequenceValue(StoreExpirySequenceKey, 1),
                    LastUpdated = DateTime.UtcNow,
                    WindowStart = DateTime.UtcNow
                };
                Provider.Set(StoreExpiryStateDataKey, Serializer.Serialize(stateData));
            }
            return stateData;
        }

        protected IEnumerable<StoreExpiryData> GetExpiryDataRow(ulong offsetWindow, out ulong cas)
        {
            var key = StoreExpiryDateRowPrefix + "-" + offsetWindow;
            return Helpers.SeparateJsonArray(Provider.Get(key, out cas)).Select(s => Serializer.Deserialize<StoreExpiryData>(s));
        }

        protected void SetExpiryDataRow(ulong offsetWindow, IEnumerable<StoreExpiryData> data, ulong cas)
        {
            var key = StoreExpiryDateRowPrefix + "-" + offsetWindow;
            Provider.Set(key, String.Concat(data.Select(s => Serializer.Serialize(s))), cas);
        }
    }

    public class StoreExpiryStateData
    {
        public ulong SequenceCurrentVal { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime WindowStart { get; set; }
    }

    public class StoreExpiryLock
    {
        public DateTime Expiry { get; set; }
        public string MachineName { get; set; }
    }

    public class StoreExpiryData
    {
        public string TargetKey { get; set; }
        public DateTime Expires { get; set; }
    }
}

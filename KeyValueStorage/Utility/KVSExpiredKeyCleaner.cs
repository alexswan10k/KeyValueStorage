using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.Utility.Data;

namespace KeyValueStorage.Utility
{
    public class KVSExpiredKeyCleaner : IExpiredKeyCleaner
    {
        public IStoreProvider Provider { get; protected set; }
        public string LockKey { get; protected set; }
        public string StoreExpirySequenceKey { get; protected set; }
        public string StoreExpiryStateDataKey { get; protected set; }
        public int LockExpiryTimeS { get; protected set; }
        public ITextSerializer Serializer { get; protected set; }
        public string StoreExpiryDateRowPrefix { get; protected set; }
        public string StoreExpiryDataExpiryKeyPrefix { get; protected set; }
        public TimeSpan WindowResolution { get; protected set; }

        public KVSExpiredKeyCleaner(IStoreProvider provider, string lockKey, TimeSpan windowResolution)
        {
            LockKey = lockKey;
            WindowResolution = windowResolution;
            LockExpiryTimeS = 320;
            Serializer = new ServiceStackTextSerializer();
            StoreExpiryDateRowPrefix = "-SE-";
            StoreExpirySequenceKey = "-SES";
            StoreExpiryStateDataKey = "-SESD";
            StoreExpiryDataExpiryKeyPrefix = "-E-";
        }

        public void CleanupKeys()
        {
            try
            {
                using (var keyLock = new KVSLockWithCAS(LockKey, DateTime.UtcNow.AddSeconds(LockExpiryTimeS), "T", Provider))
                {
                    var stateData = GetStateData();

                    //check row-1 to avoid race conditions in the unlikely event where a reference is written into window n and the sequence is moved on to n+1
                    RemoveItemsFromWindow(stateData.SequenceCurrentVal - 1);

                    //If we can clear multiple rows, do this. This will not normally occur. Allows catching up if data has not yet been processed.
                    while (RemoveItemsFromWindow(stateData.SequenceCurrentVal) == 0)
                    {
                        stateData.SequenceCurrentVal = Provider.GetNextSequenceValue(StoreExpirySequenceKey, 1);
                        stateData.LastUpdated = DateTime.UtcNow;
                        stateData.WindowStart = DateTime.UtcNow;
                        Provider.Set(StoreExpiryStateDataKey, Serializer.Serialize(stateData));
                    }
                }
            }
            catch (CASException ex)
            {
                throw new LockException(ex.Message);
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

            Provider.Append(StoreExpiryDateRowPrefix + window, Serializer.Serialize(new StoreExpiryData() { TargetKey = key, Expires = expires }));

            Provider.Set(StoreExpiryDataExpiryKeyPrefix + key, Serializer.Serialize(expires));
        }

        public DateTime GetKeyExpiry(string key)
        {
            return Serializer.Deserialize<DateTime>(Provider.Get(StoreExpiryDataExpiryKeyPrefix + key));
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
                    var compareDate = GetKeyExpiry(StoreExpiryDataExpiryKeyPrefix + item.TargetKey);
                    //If the row has somehow already been removed this will not fail.

                    //Ensure that the expiry date matches the final value (we do not want to process out of date TTL stamps)
                    if (compareDate != null && compareDate == item.Expires)
                    {
                        Provider.Remove(StoreExpiryDataExpiryKeyPrefix + item.TargetKey);
                        Provider.Remove(item.TargetKey);
                    }
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

        private ulong GetWindow(DateTime date, StoreExpiryStateData stateData)
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

        private StoreExpiryStateData GetStateData()
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

        private IEnumerable<StoreExpiryData> GetExpiryDataRow(ulong offsetWindow, out ulong cas)
        {
            var key = StoreExpiryDateRowPrefix + offsetWindow;
            return Helpers.SeparateJsonArray(Provider.Get(key, out cas)).Select(s => Serializer.Deserialize<StoreExpiryData>(s));
        }

        private void SetExpiryDataRow(ulong offsetWindow, IEnumerable<StoreExpiryData> data, ulong cas)
        {
            var key = StoreExpiryDateRowPrefix + offsetWindow;
            Provider.Set(key, String.Concat(data.Select(s => Serializer.Serialize(s))), cas);
        }
    }
}


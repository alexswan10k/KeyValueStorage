
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using Cassandra;
using KeyValueStorage.Utility;

namespace KeyValueStorage.Cassandra
{
    public class CassandraStoreProvider : IStoreProvider
    {
        public Session Session { get; set; }
        public const string KVSKeyspaceDefault = "KVSKS";
        const string KVSTableNameDefault = "KVS";
        const string LockPrefix = "-L-";

        public CassandraStoreProvider(Session session)
        {
            Session = session;
        }

        #region IStoreProvider
        public void Initialize()
        {
            Session.CreateKeyspaceIfNotExists(KVSKeyspaceDefault);
            Session.ChangeKeyspace(KVSKeyspaceDefault);
            try
            {
                Session.Execute("Create Table " + KVSTableNameDefault + " (Key text, Value text, CAS int, PRIMARY KEY(Key))");
            }
            catch (Exception ex)
            {

            }
        }

        public string Get(string key)
        {
            var row = Session.Execute("Select Value from " + KVSTableNameDefault + " where Key = '" + key + "'").GetRows().FirstOrDefault();
               
            if(row != null)
                return row.GetValue<string>(0);

            return string.Empty;
        }

        public void Set(string key, string value)
        {
            Session.Execute("Insert into " + KVSTableNameDefault + " (Key, Value, CAS) values ('" + key + "','" + value + "', 1)");
        }

        public void Remove(string key)
        {
            Session.Execute("Delete from " + KVSTableNameDefault + " Where Key = '" + key + "'");
        }

        public string Get(string key, out ulong cas)
        {
            var row = Session.Execute("Select Value, CAS from " + KVSTableNameDefault + " where Key = '" + key + "'").GetRows().FirstOrDefault();

            if (row != null)
            {
                if (row.Columns.Count() == 2)
                    cas = row.GetValue<ulong>(1);
                else
                    cas = 0;

                return row.GetValue<string>(0);
            }
            else
            {
                //set CAS even if null entry
            }
            cas = 0;
            return string.Empty;

            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas)
        {
            using (var keyLock = new KVSLockWithoutCAS(LockPrefix + key, DateTime.Now.AddSeconds(10), this))
            {

            }

            throw new NotImplementedException();
        }

        public void Set(string key, string value, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public DateTime? ExpiresOn(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountStartingWith(string key)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            throw new NotImplementedException();
            //Session.Execute()
        }

        public void Append(string key, string value)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Extensions;
using Oracle.ManagedDataAccess.Client;

namespace KeyValueStorage.Oracle
{
    public class OracleStoreProvider : IRDbStoreProvider
    {
        public IDbConnection Connection { get; protected set; }
        public bool OwnsConnection { get; protected set; }
        public string KVSTableName { get; protected set; }
        const string KVSTableNameDefault = "KVS";

        const int OracleCharLimit = 4000;
        const int JsonValueParams = 4;

        public OracleStoreProvider(global::Oracle.ManagedDataAccess.Client.OracleConnection connection)
        {
            Connection = connection;
            OwnsConnection = false;

            KVSTableName = KVSTableNameDefault;
        }

        public OracleStoreProvider(string connectionString)
        {
            Connection = new OracleConnection(connectionString);
            OwnsConnection = true;

            KVSTableName = KVSTableNameDefault;
        }

        protected void BeginOperation()
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
        }

        public bool SetupWorkingTable()
        {
            BeginOperation();
            try
            {
                StringBuilder valueCols = new StringBuilder();

                for (int i = 1; i <= JsonValueParams; i++)
                {
                    valueCols.Append("Value" + i + " Varchar2(4000),");
                }

                    Connection.ExecuteNonQuery("create table " + KVSTableName + " ("
                        + "Key Varchar2(128),"
                        //+ "Value1 Varchar2(4000),"
                        + valueCols.ToString()
                        + "Expires timestamp(6),"
                        + "CAS Number(38,0)"
                        + ")");

                Connection.ExecuteNonQuery("alter table " + KVSTableName + " add constraint PK_" + KVSTableName + "_Key primary key (Key)");
                return true;
            }
            catch (OracleException oex)
            {
                if (oex.Number != 955)
                    throw;
            }

            return false;
        }

        #region IStoreProvider
        public string Get(string key)
        {
            BeginOperation();
            try
            {
                var dt = Connection.ExecuteSql("Select Value from " + KVSTableName + " where Key = :p1", key).AsEnumerable();

                if (dt.Count() == 1)
                    return dt.First()[0] as string;
                else
                    return String.Empty;
            }
            catch (OracleException ex)
            {
                throw;
            }
        }

        public void Set(string key, string value)
        {
            BeginOperation();
            try
            {
                var count = Connection.ExecuteSql("Select Count(Value) from " + KVSTableName + " where Key = :p1", key).AsEnumerable().First()[0] as decimal?;

                if (count == 0)
                {
                    Connection.ExecuteNonQuery("Insert into " + KVSTableName + "(Key, Value) values (:p1, :p2)", key, value);
                }
                else
                {
                    var rows = Connection.ExecuteNonQuery("Update " + KVSTableName + " Set Value = :p1 where key = :p2", value, key);
                    if (rows != 1)
                        throw new Exception("Update did not affect the expected number of rows");
                }
            }
            catch (OracleException ex)
            {
                throw;
            }
        }

        public void Remove(string key)
        {
            BeginOperation();
            try
            {
                Connection.ExecuteSql("Delete from " + KVSTableName + " where key = :p1", key);
            }
            catch (OracleException ex)
            {
                throw;
            }
        }

        public string Get(string key, out ulong cas)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas)
        {
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

        public void Set(string key, string value, ulong CAS, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong CAS, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            try
            {
                var count = Connection.ExecuteSql("Select Count(Value) from " + KVSTableName + " where Key = :p1", key).AsEnumerable().First()[0] as decimal?;
                if (count > 0)
                    return true;
                return false;
            }
            catch (OracleException ex)
            {
                throw ex;
            }
        }

        public DateTime? ExpiresOn(string key)
        {
            return Connection.ExecuteSql("Select Expires from " + KVSTableName + " where Key = :p1", key).AsEnumerable().First()[0] as DateTime?;
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            return Connection.ExecuteSql("Select Value from " + KVSTableName + " where Key like '" + key + "%'").AsEnumerable().Select(s => s[0] as string);
        }

        public IEnumerable<string> GetContaining(string key)
        {
            return Connection.ExecuteSql("Select Value from " + KVSTableName + " where Key like '%" + key + "%'").AsEnumerable().Select(s => s[0] as string);
        }

        public IEnumerable<string> GetAllKeys()
        {
            return Connection.ExecuteSql("Select Key from " + KVSTableName).AsEnumerable().Select(s => s[0] as string);
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            return Connection.ExecuteSql("Select Key from " + KVSTableName + " where Key like '" + key + "%'").AsEnumerable().Select(s => s[0] as string);
        }

        public IEnumerable<string> GetKeysContaining(string key)
        {
            return Connection.ExecuteSql("Select Key from " + KVSTableName + " where Key like '%" + key + "%'").AsEnumerable().Select(s => s[0] as string);
        }

        public int CountStartingWith(string key)
        {
            var val = Connection.ExecuteSql("Select Count(Key) from " + KVSTableName + " where Key like '" + key + "%'").AsEnumerable().First()[0] as decimal?;
            return (int)val.Value;
        }

        public int CountContaining(string key)
        {
            var val = Connection.ExecuteSql("Select Count(Key) from " + KVSTableName + " where Key like '%" + key + "%'").AsEnumerable().First()[0] as decimal?;
            return (int)val.Value;
        }

        public int CountAll()
        {
            var val = Connection.ExecuteSql("Select Count(Key) from " + KVSTableName).AsEnumerable().First()[0] as decimal?;
            return (int)val.Value;
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            throw new NotImplementedException();
        }

        public void Append(string key, string value)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected void SplitAndInsert(string key, string json)
        {
            var serializedJsonSplit = json.SplitInParts(OracleCharLimit);

            Insert(
                new string[] { "Key" }.Concat(Enumerable.Range(1, JsonValueParams).Select(s => "Value" + s)),
                new object[] { key }
                    .Concat(serializedJsonSplit)
                    .Concat(Enumerable.Repeat<object>(null, JsonValueParams - serializedJsonSplit.Count()))     //pad out our collection with null params
                );
        }

        protected void Insert(IEnumerable<string> colNames, IEnumerable<object> values)
        {
            StringBuilder baseSqlCmd = new StringBuilder("Insert into [Table] ([Cols]) values ([ValueParams]);");
            baseSqlCmd.Replace("[Table]", KVSTableName);
            baseSqlCmd.Replace("[Cols]", string.Join(", ", colNames));

            //This line is creating a sequence such as :2, :3, :4, :5 to accommodate for the value parameters
            baseSqlCmd.Replace("[ValueParams]",
                string.Join(", ", Enumerable.Range(2, values.Count()).Select(s => s.ToString() + ":"))
                );
            Connection.ExecuteNonQuery(baseSqlCmd.ToString(), values);
        }

        protected void Update(IEnumerable<string> colNames, IEnumerable<object> values)
        {
            StringBuilder baseSqlCmd = new StringBuilder("Update [Table] set ([Cols]) values ([ValueParams]) Where ;");
            baseSqlCmd.Replace("[Table]", KVSTableName);
            baseSqlCmd.Replace("[Cols]", string.Join(", ", colNames));

            //This line is creating a sequence such as :2, :3, :4, :5 to accommodate for the value parameters
            baseSqlCmd.Replace("[ValueParams]",
                string.Join(", ", Enumerable.Range(2, values.Count()).Select(s => s.ToString() + ":"))
                );
            Connection.ExecuteNonQuery(baseSqlCmd.ToString(), values);
        }

        public void Dispose()
        {
            if (OwnsConnection)
                Connection.Dispose();
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
}

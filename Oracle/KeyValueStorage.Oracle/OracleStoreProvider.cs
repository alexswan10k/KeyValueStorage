using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Extensions;
using Oracle.ManagedDataAccess.Client;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage.Oracle
{
    public class OracleStoreProvider : IRDbStoreProvider
    {
        static OracleStoreProvider()
        {
            IDbConnectionSqlExtensions.SqlDialect = new Utility.SqlDialectProviderCommon();
        }

        public IDbConnection Connection { get; protected set; }
        public bool OwnsConnection { get; protected set; }
        public string KVSTableName { get; protected set; }
        const string KVSTableNameDefault = "KVS";

        const int OracleCharLimit = 4000;
        const int JsonValueParams = 10;

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
                if (oex.Number != 955) //table already exists ex
                    throw;
            }

            return false;
        }

        #region IStoreProvider
        public void Initialize()
        {
            var dialect = new Utility.SqlDialectProviderCommon();
            dialect.ParameterPrefix = ":";
            Extensions.IDbConnectionSqlExtensions.SqlDialect = dialect;
            SetupWorkingTable();
        }

        public string Get(string key)
        {
            BeginOperation();
            try
            {
                var sql = "Select [Values] from " + KVSTableName + " where Key = :p1";
                sql = sql.Replace("[Values]", GetValueColumnNames());
                var valRows = ConcatValueColumns(Connection.ExecuteSql(sql, key));
                if (valRows.Count() == 1)
                    return valRows.First();
                else
                    return string.Empty;
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
                var count = Connection.ExecuteSql("Select Count(Key) from " + KVSTableName + " where Key = :p1", key).AsEnumerable().First()[0] as decimal?;

                if (count == 0)
                {
                    SplitAndInsert(key, value);
                }
                else
                {
                    var rows = SplitAndUpdate(key, value);
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
                Connection.ExecuteSql("Delete from " + KVSTableName + " where Key = :p1", key);
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
                var count = Connection.ExecuteSql("Select Count(Key) from " + KVSTableName + " where Key = :p1", key).AsEnumerable().First()[0] as decimal?;
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
            var sql = "Select [Values] from " + KVSTableName + " where Key like '" + key + "%'";
            sql = sql.Replace("[Values]", GetValueColumnNames());
            return ConcatValueColumns(Connection.ExecuteSql(sql));
        }

        public IEnumerable<string> GetContaining(string key)
        {
            var sql = "Select [Values] from " + KVSTableName + " where Key like '%" + key + "%'";
            sql = sql.Replace("[Values]", GetValueColumnNames());
            return ConcatValueColumns(Connection.ExecuteSql(sql));
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
            //get the key's value (this is the name of the sequence)
            //execute the sequence which is referenced

            //Create a key of the same name.
            //Set its value to a seq prefix plus random unique identifier (within ora max char limit)
            //Create a sequence of the name of the key's value
        }

        public void Append(string key, string value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SqlHelpers
        protected int SplitAndInsert(string key, string json)
        {
            var serializedJsonSplit = json.SplitInParts(OracleCharLimit);

            List<ColumnValue> colVals = new List<ColumnValue>(serializedJsonSplit.Count());
            colVals.Add(new ColumnValue("Key", key));
            int i = 0;
            foreach(var item in serializedJsonSplit)
            {
                var cv = new ColumnValue();
                cv.ColumnName = "Value"+(i+1);
                cv.Value = item;
                colVals.Add(cv);
                i++;
            }

            return Connection.ExecuteInsertParams(KVSTableName, colVals.ToArray());
        }

        protected int SplitAndUpdate(string key, string json)
        {
            var serializedJsonSplit = json.SplitInParts(OracleCharLimit);

            List<ColumnValue> colVals = new List<ColumnValue>(serializedJsonSplit.Count());
            int i = 0;
            foreach (var item in serializedJsonSplit)
            {
                var cv = new ColumnValue();
                cv.ColumnName = "Value" + (i + 1);
                cv.Value = item;
                colVals.Add(cv);
                i++;
            }

            return Connection.ExecuteUpdateParams(KVSTableName, new WhereClause("Key", Operator.Equals, key), colVals.ToArray());
        }

        protected IEnumerable<string> ConcatValueColumns(DataTable table)
        {
            var outStrings = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                outStrings.Add(ConcatValueColumns(row));
            }

            return outStrings;
        }

        protected string ConcatValueColumns(DataRow row)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < JsonValueParams; i++)
            {
                var rowName = "Value" + (i+1);
                if (row.Table.Columns.Contains(rowName))
                {
                    string val = row[rowName] as string;
                    if (val != null)
                        sb.Append(val);
                }
            }
            return sb.ToString();
        }

        protected string GetValueColumnNames()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < JsonValueParams; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var rowName = "Value" + (i + 1);
                sb.Append(rowName);
            }
            return sb.ToString();
        }
        #endregion

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

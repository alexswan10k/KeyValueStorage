using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Extensions;
using System.Data.SqlClient;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.RetryStrategies;
using KeyValueStorage.Utility;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage.SqlServer
{
    public class SqlServerStoreProvider : IRDbStoreProvider
    {
        private SqlDialectProviderCommon _sqlDialect = new SqlDialectProviderCommon();
        public IDbConnection Connection { get; protected set; }
            public bool OwnsConnection { get; protected set; }
            public string KVSTableName { get; protected set; }
            const string KVSTableNameDefault = "KVS";
            const string lockPrefix = "-L-";

            public RdbExpiredKeyCleaner KeyCleaner { get; protected set; } 

            public SqlServerStoreProvider(System.Data.SqlClient.SqlConnection connection)
            {
                Connection = connection;
                OwnsConnection = false;

                KVSTableName = KVSTableNameDefault;
                KeyCleaner = new RdbExpiredKeyCleaner(this);
            }

            public SqlServerStoreProvider(string connectionString)
            {
                Connection = new System.Data.SqlClient.SqlConnection(connectionString);
                OwnsConnection = true;

                KVSTableName = KVSTableNameDefault;
                KeyCleaner = new RdbExpiredKeyCleaner(this);
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
                    Connection.ExecuteNonQuery("create table " + KVSTableName + " ("
                        + "[Key] Varchar(128) not null,"
                        + "[Value] Varchar(MAX),"
                        + "Expires datetime,"
                        + "CAS int"
                        + ")");

                    Connection.ExecuteNonQuery("Alter table "+ KVSTableName 
                        +" add constraint PK_"+ KVSTableName
                        +" primary key clustered ([Key] ASC)");

                    return true;
                }
                catch (SqlException ex)
                {
                    if(ex.ErrorCode !=-2146232060)
                        throw;
                }

                return false;
            }

            #region IStoreProvider
            public void Initialize()
            {
                SetupWorkingTable();
            }

            public string Get(string key)
            {
                BeginOperation();
                try
                {
                    var dt = Connection.ExecuteSql("Select [Value] from " + KVSTableName + " where [Key] = @p1", key).AsEnumerable();

                    if (dt.Count() == 1)
                        return dt.First()[0] as string;
                    else
                        return String.Empty;
                }
                catch (SqlException ex)
                {
                    throw;
                }
            }



            public void Set(string key, string value)
            {
                BeginOperation();
                _Set(key, value, null);
            }



            public void Remove(string key)
            {
                BeginOperation();
                try
                {
                    Connection.ExecuteSql("Delete from " + KVSTableName + " where [Key] = @p1", key);
                }
                catch (SqlException ex)
                {
                    throw;
                }
            }

            public string Get(string key, out ulong cas)
            {
                cas = 0;
                BeginOperation();
                var dt = _sqlDialect.ExecuteSelectParams(Connection, KVSTableName, new[] {"[Value]", "[Cas]"},
                                                         new WhereClause("[Key]", Operator.Equals, key));
                if (dt.Rows.Count != 1)
                    return string.Empty;

                cas = GenerateCas();
                _sqlDialect.ExecuteUpdateParams(Connection, KVSTableName,
                                                new[] {new WhereClause("[Key]", Operator.Equals, key)},
                                                new ColumnValue("[Cas]",(int) cas));

                if (dt.Rows.Count != 1)
                    return string.Empty;

                return (string) dt.Rows[0]["Value"];

                //use triggers?
                throw new NotImplementedException();
            }

        private static ulong GenerateCas()
        {
            ulong cas;
            byte[] bytes = new byte[4];
            new Random().NextBytes(bytes);
            cas = BitConverter.ToUInt16(bytes, 0);
            return cas;
        }

        public void Set(string key, string value, ulong cas)
            {
                BeginOperation();
                AssertCasIsValid(key, cas);

                _Set(key, value, null); 
            }

        private void AssertCasIsValid(string key, ulong cas)
        {
            var casExisting =(int?)
                Connection.ExecuteSql("Select [Cas] from " + KVSTableName + " where [Key] = '" + key + "'").AsEnumerable().First
                    ()[0];

            if(casExisting == null)
                throw new CASException("Key does not exist for Cas " + cas);

            var casExistingUlong = (ulong) casExisting;

            if (casExistingUlong != cas)
                throw new CASException();
        }

            public void Set(string key, string value, DateTime expires)
            {
                BeginOperation();
                _Set(key, value, expires); 
            }

            public void Set(string key, string value, TimeSpan expiresIn)
            {
                BeginOperation();
                _Set(key, value, DateTime.UtcNow + expiresIn);
            }

            public void Set(string key, string value, ulong cas, DateTime expires)
            {
                BeginOperation();
                AssertCasIsValid(key, cas);
                _Set(key, value, expires);
            }

            public void Set(string key, string value, ulong cas, TimeSpan expiresIn)
            {
                BeginOperation();
                AssertCasIsValid(key, cas);
                _Set(key, value, DateTime.UtcNow + expiresIn);
            }

            public bool Exists(string key)
            {
                try
                {
                    var count = Connection.ExecuteSql("Select Count([Value]) from " + KVSTableName + " where [Key] = @p1", key).AsEnumerable().First()[0] as decimal?;
                    if (count > 0)
                        return true;
                    return false;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }

            public DateTime? ExpiresOn(string key)
            {
                return Connection.ExecuteSql("Select Expires from " + KVSTableName + " where [Key] = @p1", key).AsEnumerable().First()[0] as DateTime?;
            }

            public IEnumerable<string> GetStartingWith(string key)
            {
                return Connection.ExecuteSql("Select [Value] from " + KVSTableName + " where [Key] like '" + key + "%'").AsEnumerable().Select(s => s[0] as string);
            }

            public IEnumerable<string> GetContaining(string key)
            {
                return Connection.ExecuteSql("Select [Value] from " + KVSTableName + " where [Key] like '%" + key + "%'").AsEnumerable().Select(s => s[0] as string);
            }

            public IEnumerable<string> GetAllKeys()
            {
                return Connection.ExecuteSql("Select [Key] from " + KVSTableName).AsEnumerable().Select(s => s[0] as string);
            }

            public IEnumerable<string> GetKeysStartingWith(string key)
            {
                return Connection.ExecuteSql("Select [Key] from " + KVSTableName + " where [Key] like '" + key + "%'").AsEnumerable().Select(s => s[0] as string);
            }

            public IEnumerable<string> GetKeysContaining(string key)
            {
                return Connection.ExecuteSql("Select [Key] from " + KVSTableName + " where [Key] like '%" + key + "%'").AsEnumerable().Select(s => s[0] as string);
            }

            public int CountStartingWith(string key)
            {
                var val = Connection.ExecuteSql("Select Count([Key]) from " + KVSTableName + " where [Key] like '" + key + "%'").AsEnumerable().First()[0] as decimal?;
                return (int)val.Value;
            }

            public int CountContaining(string key)
            {
                var val = Connection.ExecuteSql("Select Count([Key]) from " + KVSTableName + " where [Key] like '%" + key + "%'").AsEnumerable().First()[0] as decimal?;
                return (int)val.Value;
            }

            public int CountAll()
            {
                var val = Connection.ExecuteSql("Select Count([Key]) from " + KVSTableName).AsEnumerable().First()[0] as decimal?;
                return (int)val.Value;
            }

            public ulong GetNextSequenceValue(string key, int increment)
            {
                //set up a sproc for this
                using (IKeyLock keyLock = new KVSLockWithCAS(lockPrefix + key, DateTime.UtcNow.AddSeconds(10),this, retryingLock:true))
                {
                    ulong value = 0;
                    var valueRaw = Get(key);

                    if (valueRaw != string.Empty)
                        value = (ulong)int.Parse(valueRaw);

                    value = value + (ulong)increment;
                    Set(key, value.ToString());
                    return value;
                }
            }

            public void Append(string key, string value)
            {
                //This could be done far more efficiently and atomically with a sproc
                using (IKeyLock keyLock = new KVSLockWithCAS(lockPrefix + key, DateTime.UtcNow.AddSeconds(10), this, retryingLock:true))
                {
                    var existingValue = Get(key);
                    Set(key, existingValue + value);
                }
            }

        public IRetryStrategy GetDefaultRetryStrategy()
        {
            return new SimpleRetryStartegy(5, 1000);
        }

        #endregion

            private void _Set(string key, string value, DateTime? expires)
            {
                try
                {
                    var count = (int)Connection.ExecuteSql("Select Count([Value]) from " + KVSTableName + " where [Key] = @p1", key).AsEnumerable().First()[0];

                    if (count == 0)
                    {
                        Connection.ExecuteNonQuery("Insert into " + KVSTableName + "([Key], [Value], [Expires], [Cas]) values (@p1, @p2, @p3, @p4)", key, value, expires, (int)GenerateCas());
                    }
                    else
                    {
                        var rows = Connection.ExecuteNonQuery("Update " + KVSTableName + " Set [Value] = @p1, [Expires] = @p2, [Cas] = @p3 where [Key] = @p4", value, expires, 
                            (int)GenerateCas(), key);
                        if (rows != 1)
                            throw new Exception("Update did not affect the expected number of rows");
                    }
                }
                catch (SqlException ex)
                {
                    throw;
                }
            }

            public void Dispose()
            {
                if (OwnsConnection)
                    Connection.Dispose();
            }



    }
}

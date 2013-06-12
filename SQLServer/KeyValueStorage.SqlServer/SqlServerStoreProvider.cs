using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Extensions;
using System.Data.SqlClient;

namespace KeyValueStorage.SqlServer
{
    public class SqlServerStoreProvider : IRDbStoreProvider
    {
            public IDbConnection Connection { get; protected set; }
            public bool OwnsConnection { get; protected set; }
            public string KVSTableName { get; protected set; }
            const string KVSTableNameDefault = "KVS";

            public SqlServerStoreProvider(System.Data.SqlClient.SqlConnection connection)
            {
                Connection = connection;
                OwnsConnection = false;

                KVSTableName = KVSTableNameDefault;
            }

            public SqlServerStoreProvider(string connectionString)
            {
                Connection = new System.Data.SqlClient.SqlConnection(connectionString);
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
                try
                {
                    var count = (int)Connection.ExecuteSql("Select Count([Value]) from " + KVSTableName + " where [Key] = @p1", key).AsEnumerable().First()[0];

                    if (count == 0)
                    {
                        Connection.ExecuteNonQuery("Insert into " + KVSTableName + "([Key], [Value]) values (@p1, @p2)", key, value);
                    }
                    else
                    {
                        var rows = Connection.ExecuteNonQuery("Update " + KVSTableName + " Set [Value] = @p1 where [Key] = @p2", value, key);
                        if (rows != 1)
                            throw new Exception("Update did not affect the expected number of rows");
                    }
                }
                catch (SqlException ex)
                {
                    throw;
                }
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
                //use triggers?
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
                throw new NotImplementedException();
            }

            public void Append(string key, string value)
            {
                throw new NotImplementedException();
            }

            #endregion

            public void Dispose()
            {
                if (OwnsConnection)
                    Connection.Dispose();
            }



    }
}

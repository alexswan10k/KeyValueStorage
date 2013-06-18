using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Extensions
{
    public static class IDbConnectionExtensions
    {
        public static int ExecuteNonQuery(this IDbConnection connection, string sql, params object[] parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            int counter = 1;
            foreach (var param in parameters)
            {
                var par = cmd.CreateParameter();
                par.ParameterName = "p" + counter;
                par.Value = param;
                cmd.Parameters.Add(par);
                counter++;
            }

            return cmd.ExecuteNonQuery();
        }

        public static DataTable ExecuteSql(this IDbConnection connection, string sql, params object[] parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            int counter = 1;
            foreach (var param in parameters)
            {
                var par = cmd.CreateParameter();
                par.ParameterName = "p" + counter;
                par.Value = param;
                cmd.Parameters.Add(par);
                counter++;
            }

            var dt = new DataTable();
           
            using (var reader = cmd.ExecuteReader())
            {
                dt.Load(reader);
            }

            return dt;
        }
    }
}

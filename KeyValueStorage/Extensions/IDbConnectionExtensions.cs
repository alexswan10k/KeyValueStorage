using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Extensions
{
    public static class IDbConnectionExtensions
    {
        public static int ExecuteNonQuery(this IDbConnection connection, string sql, object p1 = null, object p2 = null)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            if (p1 != null)
            {
                var par1 = cmd.CreateParameter();
                par1.ParameterName = "p1";
                par1.Value = p1;
                cmd.Parameters.Add(par1);
            }
            if (p2 != null)
            {
                var par2 = cmd.CreateParameter();
                par2.ParameterName = "p2";
                par2.Value = p2;
                cmd.Parameters.Add(par2);
            }
            return cmd.ExecuteNonQuery();
        }

        public static DataTable ExecuteSql(this IDbConnection connection, string sql, object p1 = null, object p2 = null)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            if (p1 != null)
            {
                var par1 = cmd.CreateParameter();
                par1.ParameterName = "p1";
                par1.Value = p1;
                cmd.Parameters.Add(par1);
            }
            if (p2 != null)
            {
                var par2 = cmd.CreateParameter();
                par2.ParameterName = "p2";
                par2.Value = p2;
                cmd.Parameters.Add(par2);
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

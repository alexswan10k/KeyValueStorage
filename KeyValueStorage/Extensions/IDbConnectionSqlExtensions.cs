using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage.Extensions
{
    public static class IDbConnectionSqlExtensions
    {
        public static ISqlDialectProvider SqlDialect { get; set; }

        public static int ExecuteInsert(this IDbConnection connection, string tableName, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteInsert(connection, tableName, values);
        }

        public static DataTable ExecuteSelect(this IDbConnection connection, string tableName, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteSelect(connection, tableName, null, whereClauses);
        }

        public static DataTable ExecuteSelect(this IDbConnection connection, string tableName, IEnumerable<string> colNames, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteSelect(connection, tableName, colNames, whereClauses);
        }

        public int ExecuteUpdate(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteUpdate(connection, tableName, whereClauses, values);
        }

        public int ExecuteUpdate(IDbConnection connection, string tableName, WhereClause whereClause, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteUpdate(connection, tableName, new WhereClause[]{ whereClause}, values);
        }

        public int ExecuteUpdate(IDbConnection connection, string tableName, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteUpdate(connection, tableName, new WhereClause[] { }, values);
        }

        public int ExecuteDelete(IDbConnection connection, string tableName, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteDelete(connection, tableName, whereClauses);
        }
    }
}

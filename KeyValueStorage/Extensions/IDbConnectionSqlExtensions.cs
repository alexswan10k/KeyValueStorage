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

        public static int ExecuteInsertParams(this IDbConnection connection, string tableName, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteInsertParams(connection, tableName, values);
        }

        public static DataTable ExecuteSelectParams(this IDbConnection connection, string tableName, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteSelectParams(connection, tableName, null, whereClauses);
        }

        public static DataTable ExecuteSelectParams(this IDbConnection connection, string tableName, IEnumerable<string> colNames, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteSelectParams(connection, tableName, colNames, whereClauses);
        }

        public static int ExecuteUpdateParams(this IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteUpdateParams(connection, tableName, whereClauses, values);
        }

        public static int ExecuteUpdateParams(this IDbConnection connection, string tableName, WhereClause whereClause, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteUpdateParams(connection, tableName, new WhereClause[]{ whereClause}, values);
        }

        public static int ExecuteUpdateParams(this IDbConnection connection, string tableName, params ColumnValue[] values)
        {
            return SqlDialect.ExecuteUpdateParams(connection, tableName, new WhereClause[] { }, values);
        }

        public static int ExecuteDeleteParams(this IDbConnection connection, string tableName, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteDeleteParams(connection, tableName, whereClauses);
        }
    }
}

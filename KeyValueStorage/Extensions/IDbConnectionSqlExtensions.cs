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

        public static void ExecuteInsert(this IDbConnection connection, string tableName, params ColumnValue[] values)
        {
            SqlDialect.ExecuteInsert(connection, tableName, values);
        }

        public static DataTable ExecuteSelect(this IDbConnection connection, string tableName, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteSelect(connection, tableName, whereClauses);
        }

        public static DataTable ExecuteSelect(this IDbConnection connection, string tableName, IEnumerable<string> colNames, params WhereClause[] whereClauses)
        {
            return SqlDialect.ExecuteSelect(connection, tableName, whereClauses);
        }
    }
}

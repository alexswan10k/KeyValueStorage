
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage.Interfaces.Utility
{
    public interface ISqlDialectProvider
    {
        DataTable ExecuteSelectParams(IDbConnection connection, string tableName, IEnumerable<string> colNames, IEnumerable<WhereClause> whereClauses);
        int ExecuteInsertParams(IDbConnection connection, string tableName, IEnumerable<ColumnValue> values);
        int ExecuteUpdateParams(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses, IEnumerable<ColumnValue> values);
        int ExecuteDeleteParams(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses);
    }
}

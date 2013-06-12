
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
        DataTable ExecuteSelect(IDbConnection connection, string tableName, IEnumerable<string> colNames, IEnumerable<WhereClause> whereClauses);
        int ExecuteInsert(IDbConnection connection, string tableName, IEnumerable<ColumnValue> values);
        int ExecuteUpdate(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses, IEnumerable<ColumnValue> values);
        int ExecuteDelete(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses);
    }
}

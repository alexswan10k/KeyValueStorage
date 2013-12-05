
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
        DataTable ExecuteSelectParams(IDbConnection connection, string tableName, IEnumerable<string> colNames, params WhereClause[] whereClauses);
        int ExecuteInsertParams(IDbConnection connection, string tableName, params ColumnValue[] values);
        int ExecuteUpdateParams(IDbConnection connection, string tableName, WhereClause[] whereClauses, params ColumnValue[] values);
        int ExecuteDeleteParams(IDbConnection connection, string tableName, params WhereClause[] whereClauses);
    }
}

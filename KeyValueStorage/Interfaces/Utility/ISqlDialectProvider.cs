
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
        DataTable ExecuteSelect(IDbConnection connection, string tableName, params WhereClause[] whereClauses);
        DataTable ExecuteSelect(IDbConnection connection, string tableName, IEnumerable<string> colNames, params WhereClause[] whereClauses);
        void ExecuteInsert(IDbConnection connection, string tableName, params ColumnValue[] values);
    }
}

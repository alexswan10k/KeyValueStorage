
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Sql
{
    public class WhereClause
    {
        public string ColumnName { get; set; }
        public Operator Operator { get; set; }
        public object ParameterValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClause"/> class.
        /// </summary>
        public WhereClause()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClause"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="op">The op.</param>
        /// <param name="paramValue">The param value.</param>
        public WhereClause(string columnName, Operator op, object paramValue)
        {
            ColumnName = columnName;
            Operator = op;
            ParameterValue = paramValue;
        }
    }
}

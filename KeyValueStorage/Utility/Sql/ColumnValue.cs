using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility.Sql
{
    public class ColumnValue
    {
        public string ColumnName { get; set; }
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnValue"/> class.
        /// </summary>
        public ColumnValue()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnValue"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        public ColumnValue(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }
    }
}

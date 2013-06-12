using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KeyValueStorage.Extensions;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage.Utility
{
    public class SqlDialectProviderCommon : ISqlDialectProvider
    {
        protected string SelectStatementTemplate = "Select [Cols] from [Table] [Where]";
        protected string InsertStatementTemplate = "Insert into [Table] ([Cols]) values ([ValueParams])";
        protected string UpdateStatementTemplate = "Update [Table] Set [UpdateCols] [Where]";
        protected string DeleteStatementTemplate = "Delete from [Table] [Where]";
        protected string ParameterPrefix = ":";

        /// <summary>
        /// Executes an insert statement with a list of values which will be passed as parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="values">The column values to insert.</param>
        public virtual void ExecuteInsert(IDbConnection connection, string tableName, IEnumerable<ColumnValue> values)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder baseSqlCmd = new StringBuilder(InsertStatementTemplate);
            baseSqlCmd.Replace("[Table]", tableName);
            baseSqlCmd.Replace("[Cols]", string.Join(", ", values.Select(s => s.ColumnName)));

            List<Tuple<string, object>> inputParams = new List<Tuple<string, object>>();

            var cmd = connection.CreateCommand();

            for (int i = 0; i < values.Count(); i++)
            {
                var par = cmd.CreateParameter();
                par.ParameterName = ParameterPrefix + (i + 1);
                par.Value = values.ElementAt(i).Value;
                cmd.Parameters.Add(par);
            }

            //This line is creating a sequence such as :1, :2, :3 to accommodate for the value parameters
            baseSqlCmd.Replace("[ValueParams]",
                string.Join(", ", Enumerable.Range(1, values.Count()).Select(s => ParameterPrefix + s.ToString()))
                );

            cmd.CommandText = baseSqlCmd.ToString();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes an update statement with a list of values which will be passed as parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="values">The column values to insert.</param>
        public virtual int ExecuteUpdate(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses, IEnumerable<ColumnValue> values)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder baseSqlCmd = new StringBuilder(UpdateStatementTemplate);
            baseSqlCmd.Replace("[Table]", tableName);

            List<Tuple<string, object>> inputParams = new List<Tuple<string, object>>();

            StringBuilder columnSetSql = new StringBuilder("(");
            var cmd = connection.CreateCommand();
            int i = 0;
            for (; i < values.Count(); i++)
            {
                if (i > 0)
                    columnSetSql.Append(", ");

                var currentVal = values.ElementAt(i);
                var par = cmd.CreateParameter();
                par.ParameterName = ParameterPrefix + (i + 1);
                par.Value = currentVal.Value;
                cmd.Parameters.Add(par);
                columnSetSql.Append(currentVal.ColumnName + " = " + par.ParameterName);
            }
            columnSetSql.Append(")");

            baseSqlCmd.Replace("[UpdateCols]", columnSetSql.ToString());


            StringBuilder colWhereSql = WhereBuilder(cmd, whereClauses, i);

            baseSqlCmd.Replace("[Where]", colWhereSql.ToString());

            //This line is creating a sequence such as :1, :2, :3 to accommodate for the value parameters
            var paramsString = Enumerable.Range(1, values.Count()).Select(s => ParameterPrefix + s.ToString());

            cmd.CommandText = baseSqlCmd.ToString();
            return cmd.ExecuteNonQuery();
        }

                /// <summary>
        /// Executes a delete statement with a list of values which will be passed as parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="values">The column values to insert.</param>
        public virtual int ExecuteDelete(IDbConnection connection, string tableName, IEnumerable<WhereClause> whereClauses)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var cmd = connection.CreateCommand();
            StringBuilder baseSqlCmd = new StringBuilder(DeleteStatementTemplate);
            baseSqlCmd.Replace("[Table]", tableName);

            StringBuilder sbWhereClauses = WhereBuilder(cmd, whereClauses);
            baseSqlCmd.Replace("[Where]", sbWhereClauses.ToString());

            cmd.CommandText = baseSqlCmd.ToString();

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a select statement with an optional number of conditional parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="colNames">The col names.</param>
        /// <param name="whereClauses">The where clauses.</param>
        /// <returns>A data table object containing the rows retrieved from the selection.</returns>
        public virtual DataTable ExecuteSelect(IDbConnection connection, string tableName, IEnumerable<string> colNames, IEnumerable<WhereClause> whereClauses)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder baseSqlCmd = new StringBuilder(SelectStatementTemplate);

            if (colNames != null)
                baseSqlCmd.Replace("[Cols]", string.Join(", ", colNames));
            else
                baseSqlCmd.Replace("[Cols]", "*");

            baseSqlCmd.Replace("[Table]", tableName);

            var cmd = connection.CreateCommand();

            StringBuilder sbWhereClauses = WhereBuilder(cmd, whereClauses);

            baseSqlCmd.Replace("[Where]", sbWhereClauses.ToString());
            cmd.CommandText = baseSqlCmd.ToString();

            DataTable dt = new DataTable();
            using (var reader = cmd.ExecuteReader())
            {
                dt.Load(reader);
            }

            return dt;
        }

        protected virtual StringBuilder WhereBuilder(IDbCommand cmd, IEnumerable<WhereClause> whereClauses, int startAt = 1)
        {
            var colWhereSql = new StringBuilder("Where ");
            int i = 0;
            foreach (var whereClause in whereClauses)
            {
                if (i > 0)
                    colWhereSql.Append(" And ");

                var par = cmd.CreateParameter();
                par.ParameterName = ParameterPrefix + (startAt + i);
                colWhereSql.Append(GetConditionalSql(whereClause, par.ParameterName));

                par.Value = whereClause.ParameterValue;
                cmd.Parameters.Add(par);
                i++;
            }

            return colWhereSql;
        }


        /// <summary>
        /// Gets the conditional SQL subcomponent from a WhereClause object.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="parameterSub">The parameter sub.</param>
        /// <returns></returns>
        protected virtual string GetConditionalSql(WhereClause whereClause, string parameterSub)
        {
            string op;
            switch (whereClause.Operator)
            {
                case Operator.GreaterThan: op = ">"; break;
                case Operator.LessThan: op = "<"; break;
                case Operator.Equals: op = "="; break;
                default: op = ""; break;
            }

            return string.Join(" ", whereClause.ColumnName, op, parameterSub);
        }
    }
}

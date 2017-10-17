using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using StringUtils;

namespace DataTableUtils
{
    /// <summary>
    /// Static class for DataTable conversion.
    /// </summary>
    public static class DataTableConverter
    {
        /// <summary>
        /// Create a Delimiter-Separated Value representation of a DataTable. (CSV by default.)
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        /// <param name="separator">Optional delimiter.</param>
        /// <param name="quotingchar">Optional text qualifier.</param>
        /// <param name="linebreakreplacement">Optional character by which to replace newlines.</param>
        /// <returns>String containing a DSV representation of the DataTable</returns>
        public static String dataTableToDSV(DataTable dt, char separator = ',', char? quotingchar = '"', char? linebreakreplacement = null)
        {
            StringBuilder sb = new StringBuilder();
            if (dt != null)
            {
                // set up the function to prepare the raw data for embedment in CSV
                Func<String, String> CSVify;
                if (quotingchar == null)
                {
                    if (linebreakreplacement == null)
                        CSVify = (String s) => s;
                    else
                        CSVify = (String s) => Newlines.escapeNewlines(s, linebreakreplacement.ToString());
                }
                else
                {
                    String quotingString = quotingchar.ToString();
                    String escapedQuote = String.Concat(quotingchar, quotingchar);
                    if (linebreakreplacement == null)
                        CSVify = (String s) => s.Replace(quotingString, escapedQuote);
                    else
                        CSVify = (String s) => Newlines.escapeNewlines(s, linebreakreplacement.ToString()).Replace(quotingString, escapedQuote);
                }

                // set up some useful variables
                String colSeparator = String.Concat(quotingchar, separator, quotingchar);
                int trailingLength = String.Concat(separator, quotingchar).Length;

                // string together the headers
                sb.Append(quotingchar);
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    sb.Append(CSVify(dt.Columns[col].ColumnName));
                    sb.Append(colSeparator);
                }
                sb.Remove(sb.Length - trailingLength, trailingLength);
                sb.AppendLine();

                // collate all rows
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sb.Append(quotingchar);
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        sb.Append(CSVify(dt.Rows[row][col].ToString()));
                        sb.Append(colSeparator);
                    }
                    sb.Remove(sb.Length - trailingLength, trailingLength);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Helper function to map a CLR data type to a MySql data type.
        /// </summary>
        /// <param name="t">The CLR data type.</param>
        /// <returns>The corresponding MySql data type and a bool indicating whether contents in this type should be quoted.</returns>
        private static Tuple<String, bool> clrToMySqlDataType(Type t)
        {
            // http://www.alinq.org/ALinq_Document/1.Getting_Started/5.CLR_Type_AND_SQL_Type_Default_Mapping.aspx
            switch (t.ToString())
            {
                case "System.Boolean":
                    return new Tuple<String, bool>("BIT", false);
                case "System.Byte":
                    return new Tuple<String, bool>("TINYINT", false);
                case "System.Byte[]":
                    return new Tuple<String, bool>("BINARY", false);
                case "System.Char":
                    return new Tuple<String, bool>("CHAR(1)", true);
                case "System.DateTime":
                    return new Tuple<String, bool>("DATETIME", true);
                case "System.Decimal":
                    return new Tuple<String, bool>("REAL", false);
                case "System.Double":
                    return new Tuple<String, bool>("FLOAT", false);
                case "System.Guid":
                    return new Tuple<String, bool>("TINYBLOB", true);
                case "System.Int16":
                case "System.UInt16":
                case "System.SByte":
                    return new Tuple<String, bool>("SMALLINT", false);
                case "System.Int32":
                case "System.UInt32":
                    return new Tuple<String, bool>("INT", false);
                case "System.Int64":
                case "System.UInt64":
                    return new Tuple<String, bool>("BIGINT", false);
                case "System.Single":
                    return new Tuple<String, bool>("REAL", false);
                case "System.String":
                    return new Tuple<String, bool>("TEXT", true);
                default:
                    throw new Exception(t.ToString() + " not implemented.");
            }
        }

        /// <summary>
        /// Creates a MySql query for the creation and population of a database representing the DataTable.
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        /// <param name="tableName">A MySql table name.</param>
        /// <param name="quoteEntity">Optional character with which to quote MySql entities.</param>
        /// <param name="quoteText">Optional character with which to quote text and other quotable column contents.</param>
        /// <returns>A String containing a query to create and populate a MySql database representing the DataTable.</returns>
        public static String dataTableToMySql(DataTable dt, String tableName, char quoteEntity = '`', char quoteText = '\'')
        {
            StringBuilder sb = new StringBuilder();
            if (dt != null)
            {
                // initialize some constant values
                String quotSingle = quoteText.ToString();
                String quotDouble = new String(quoteText, 2);
                Func<String, String> escapeQuotes = (String s) => s.Replace(quotSingle, quotDouble);
                String insertStatementStart = String.Format("INSERT INTO {0}{1}{0} VALUES(", quoteEntity, tableName);

                // construct CREATE statement (with extra column, containing row numbers, as primary key)
                sb.Append(String.Format("CREATE TABLE {0}{1}{0} ({0}PK_ID{0} {2} NOT NULL PRIMARY KEY,", quoteEntity, tableName, clrToMySqlDataType(typeof(int)).Item1));
                bool[] escapeText = new bool[dt.Columns.Count];
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    Tuple<String, bool> coltype = clrToMySqlDataType(dt.Columns[col].DataType);
                    escapeText[col] = coltype.Item2;
                    sb.Append(String.Format("{0}{1}{0} {2}, ", quoteEntity, dt.Columns[col].ColumnName, coltype.Item1));
                }
                sb.Remove(sb.Length - 2, 2);
                sb.AppendLine(")  CHARACTER SET utf8 COLLATE utf8_unicode_ci;");

                // INSERT rows
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sb.Append(insertStatementStart).Append((row + 1).ToString()).Append(',');
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        String s = dt.Rows[row][col].ToString().Replace(@"\", @"\\");
                        if (escapeText[col])
                            sb.Append(quoteText).Append(escapeQuotes(s)).Append(quoteText);
                        else
                            sb.Append(s);
                        sb.Append(',');
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.AppendLine(");");
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Class supplying generic operations for a DataTable.
    /// </summary>
    /// <typeparam name="T">A class type.</typeparam>
    public static class DealWithIt<T> where T : class
    {
        /// <summary>
        /// Method for applying custom functions to every field in a DataTable.
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        /// <param name="f">Unary function to apply to the contents of a field.</param>
        /// <param name="g">Binary function that concatenates (or otherwise combines) the previous result(s) of function <c>f</c> with a new result.</param>
        /// <returns>End result of the <c>g</c> concatenation of the <c>f</c> function applied to each field in the DataTable.</returns>
        public static T walkAllThrough(DataTable dt, Func<String, T> f, Func<T, T, T> g)
        {
            T t = null;
            for (int row = 0; row < dt.Rows.Count; row++)
                for (int col = 0; col < dt.Columns.Count; col++)
                    t = g(t, f(dt.Rows[row][col].ToString()));
            return t;
        }
    }
}

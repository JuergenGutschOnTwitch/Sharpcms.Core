//// sharpcms is licensed under the open source license GPL - GNU General Public License.

//using System.Collections.Generic;
//using System.Data;
//using System.Data.OleDb;
//using System.Linq;
//using System.Text;
//using System.Xml;

//namespace Sharpcms.Base.Library
//{
//    public class DatabaseConnection
//    {
//        private readonly string _connectionString;

//        public DatabaseConnection(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public void GetXml(string query, XmlNode xmlNode)
//        {
//            GetXml(query, xmlNode, "result");
//        }

//        private void GetXml(string query, XmlNode xmlNode, string tableName, string rowName = "row")
//        {
//            OleDbConnection connection = new OleDbConnection(_connectionString);
//            connection.Open();

//            DataSet dataSet = new DataSet();
//            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, connection);
//            dataAdapter.Fill(dataSet);

//            connection.Close();

//            // Better naming
//            dataSet.DataSetName = tableName;
//            dataSet.Tables[0].TableName = rowName;

//            xmlNode.InnerXml = dataSet.GetXml();
//        }

//        private static string QuoteValue(string value)
//        {
//            string quoteValue = value.Replace("'", "''");

//            return quoteValue;
//        }

//        public DataTable GetDataTable(string query, params string[] values)
//        {
//            object[] quotedValues = new object[values.Length];

//            for (int i = 0; i < values.Length; i++)
//            {
//                quotedValues[i] = QuoteValue(values[i]);
//            }

//            string newQuery = string.Format(query, quotedValues);

//            DataTable dataTable;
//            using (OleDbConnection connection = new OleDbConnection(_connectionString))
//            {
//                connection.Open();

//                dataTable = new DataTable();
//                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(newQuery, connection);
//                dataAdapter.Fill(dataTable);

//                connection.Close();
//            }

//            return dataTable;
//        }

//        private object ExecuteScalar(string query, params string[] values)
//        {
//            object[] quotedValues = new object[values.Length];

//            for (int i = 0; i < values.Length; i++)
//            {
//                quotedValues[i] = QuoteValue(values[i]);
//            }

//            string newQuery = string.Format(query, quotedValues);

//            OleDbConnection connection = new OleDbConnection(_connectionString);
//            connection.Open();

//            OleDbCommand command = new OleDbCommand(newQuery, connection);
//            object value = command.ExecuteScalar();

//            connection.Close();

//            return value;
//        }

//        private void ExecuteNonQuery(string query)
//        {
//            OleDbConnection connection = new OleDbConnection(_connectionString);
//            connection.Open();

//            OleDbCommand command = new OleDbCommand(query, connection);
//            command.ExecuteNonQuery();

//            connection.Close();
//        }

//        public void SaveXml(XmlNode dataNode, string tableName, string primaryKey)
//        {
//            List<string> columns = GetColumnsFromNode(dataNode);

//            foreach (XmlNode childNode in dataNode.ChildNodes)
//            {
//                // Get values
//                Dictionary<string, string> values = GetValuesFromNode(columns, childNode);

//                // First, check to see if a record exists
//                int recordCount = (int) ExecuteScalar("SELECT COUNT(*) AS RecordCount FROM {0} WHERE {1} = '{2}'", tableName, primaryKey, values[primaryKey]);
//                if (recordCount > 0)
//                {
//                    SaveXmlByUpdate(tableName, primaryKey, columns, values);
//                }
//                else
//                {
//                    SaveXmlByInsert(tableName, columns, values);
//                }
//            }
//        }

//        public string ConvertToSafeString(string txt)
//        {
//            string safeString = txt.Replace("'", "''");

//            return safeString;
//        }

//        #region Helper methods for SaveXml

//        private List<string> GetColumnsFromNode(XmlNode dataNode)
//        {
//            List<string> columns = new List<string>();

//            // Get list of columns
//            if (dataNode.ChildNodes.Count > 0)
//            {
//                XmlNode firstChild = dataNode.FirstChild;
//                columns.AddRange(from XmlNode rowNode in firstChild.ChildNodes select rowNode.Name);
//            }

//            return columns;
//        }

//        private static Dictionary<string, string> GetValuesFromNode(IEnumerable<string> columns, XmlNode childNode)
//        {
//            Dictionary<string, string> values = new Dictionary<string, string>();
//            foreach (string column in columns)
//            {
//                string value = string.Empty;

//                try
//                {
//                    XmlNode selectSingleNode = childNode.SelectSingleNode(column);
//                    if (selectSingleNode != null)
//                    {
//                        value = selectSingleNode.InnerText;
//                    }
//                }
//                catch
//                {
//                    value = string.Empty;
//                }

//                values[column] = value;
//            }

//            return values;
//        }

//        private void SaveXmlByInsert(string tableName, List<string> columns, Dictionary<string, string> values)
//        {
//            StringBuilder queryBuilder = new StringBuilder();
//            queryBuilder.AppendFormat("INSERT INTO {0} (", tableName);
//            queryBuilder.Append(string.Join(", ", columns.ToArray()));
//            queryBuilder.Append(") VALUES (");
//            queryBuilder.Append(string.Join(", ", columns.Select(column => string.Format("'{0}'", QuoteValue(values[column]))).ToArray()));
//            queryBuilder.Append(")");

//            ExecuteNonQuery(queryBuilder.ToString());
//        }

//        private void SaveXmlByUpdate(string tableName, string primaryKey, IEnumerable<string> columns, Dictionary<string, string> values)
//        {
//            // Run an UPDATE command
//            StringBuilder queryBuilder = new StringBuilder();
//            queryBuilder.AppendFormat("UPDATE {0} SET ", tableName);
//            queryBuilder.Append(string.Join(", ", columns.Select(column => string.Format("{0} = '{1}'", column, QuoteValue(values[column]))).ToArray()));
//            queryBuilder.AppendFormat(" WHERE {0} = '{1}'", primaryKey, QuoteValue(values[primaryKey]));

//            ExecuteNonQuery(queryBuilder.ToString());
//        }

//        #endregion
//    }
//}
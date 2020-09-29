namespace ToDoHelperLibrary
{
    using System;
    using System.Data;
    using MySql.Data.MySqlClient;

    /// <summary>
    ///Class represents DB Communication
    /// This class uses Singleton pattern to create instance of DbCommunicator
    /// </summary>
    public class DbCommunicator
    {
        private string _connectionString;
        private static DbCommunicator _instance;
        private MySqlConnection _connection;

        /// <summary>
        /// Privately instantiate the DbCommunication.
        /// </summary>
        /// <param name="connectionString"></param>
        private DbCommunicator(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Method to get instance using singlton pattern. Create instance only once.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbCommunicator CreateInstance(string connectionString)
        {
            if (null == _instance)
            {
                _instance = new DbCommunicator(connectionString);
            }
            else
            {
                // Do nothing
            }
            return _instance;
        }

        /// <summary>
        /// Open connection with database.
        /// </summary>
        public void OpenConnection()
        {
            _connection = new MySqlConnection(_connectionString);
            _connection.Open();
        }

        /// <summary>
        /// Close connection with database
        /// </summary>
        public void CloseConnection()
        {
            if (null != _connection)
            {
                _connection.Close();
            }
            else
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Execute query to get data
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string query, string tableName)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            MySqlCommand cmd = new MySqlCommand(query, _connection);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataSet, tableName);
                dataTable = dataSet.Tables[tableName];
                return dataSet;
            }
        }

        /// <summary>
        /// Execute query to return rows affected.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, _connection);
            int numberOfRows = cmd.ExecuteNonQuery();
            return numberOfRows;
        }

        /// <summary>
        /// Execute query to get scalar data like count of query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int ExecuteScalar(string query)
        {
            Int32 numberOfRows = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                object result = cmd.ExecuteScalar();
                // If result is DB null or empty then set number of Rows to 0.
                if (null == result || DBNull.Value.Equals(result))
                {
                    numberOfRows = 0;
                }
                else
                {
                    numberOfRows = Convert.ToInt32(result);
                }
            }
            catch (FormatException)
            {
                numberOfRows = 1;
            }
            return numberOfRows;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace VDBS.App_Code
{
    public class DatabaseUtiltiy
    {
        private string _connectionString = string.Empty;
        public string GetConnectionString()
        {
            return _connectionString;
        }
        public DatabaseUtiltiy()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["con"].ToString();
        }

        #region Select Commands (5)

        public DataTable ExecuteReader(string query)
        {
            if (string.IsNullOrEmpty(query))
                return null;
            else
                return ExecuteReader(new SqlCommand(query));
        }
        public DataTable ExecuteReaderWithParameters(string sqlQuery, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sqlQuery) || parameters == null || parameters.Keys.Count == 0)
                return null;
            else
            {
                var command = new SqlCommand();
                command.CommandText = sqlQuery;

                foreach (string key in parameters.Keys)
                {
                    command.Parameters.Add(new SqlParameter(key, parameters[key]));
                }

                return ExecuteReader(command);
            }
        }
        public string ExecuteReaderScalar(string query)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                var dbResponse = ExecuteReader(query);

                if (dbResponse != null && dbResponse.Rows.Count > 0)
                    response = dbResponse.Rows[0][0].ToString();
            }
            return response;
        }
        public string ExecuteReaderScalarWithParameters(string query, Dictionary<string, object> parameters)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(query) && parameters != null && parameters.Count > 0)
            {
                var dbResponse = ExecuteReaderWithParameters(query, parameters);

                if (dbResponse != null && dbResponse.Rows.Count > 0)
                    response = dbResponse.Rows[0][0].ToString();
            }
            return response;
        }
        public DataTable ExecuteReader(SqlCommand command)
        {
            if (command == null)
                return null;
            else
            {
                DataTable dt = new DataTable();
                var connection = new SqlConnection();
                try
                {

                    connection.ConnectionString = GetConnectionString();
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Open();
                    command.Connection = connection;
                    var da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }
                catch (SqlException oraExp)
                {
                    LogMessageToFile(oraExp.Message, oraExp.StackTrace);
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                finally
                {
                    command.Dispose();

                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
                return dt;
            }
        }

        #endregion

        #region Non Quries (Insert, Update, Delete) Commands (6)
        public int ExecuteNonQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
                return 0;
            else
                return ExecuteNonQuery(new SqlCommand(query));
        }
        public bool ExecuteNonQueryMultiple(List<string> queries)
        {
            if (queries == null || queries.Count == 0)
                return false;
            else
            {
                var commands = new List<SqlCommand>();

                foreach (string query in queries)
                {
                    commands.Add(new SqlCommand(query));
                }
                return ExecuteNonQueryWithMultiple(commands);
            }
        }
        public int ExecuteNonQueryWithParameters(string query, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(query) || parameters == null || parameters.Count == 0)
                return 0;
            else
            {
                var command = new SqlCommand(query);
                foreach (string key in parameters.Keys)
                {
                    command.Parameters.Add(new SqlParameter(key, parameters[key]));
                }

                return ExecuteNonQuery(command);
            }
        }
        public bool ExecuteNonQueryWithParametersMultiple(List<string> queries, List<Dictionary<string, object>> parameterList)
        {
            if (queries == null || queries.Count == 0 || parameterList == null || parameterList.Count == 0 || queries.Count != parameterList.Count)
                return false;
            else
            {
                bool validRequest = true;
                var commands = new List<SqlCommand>();
                SqlCommand temp = null;
                for (int index = 0; index < queries.Count; index++)
                {
                    if (string.IsNullOrEmpty(queries[index]) || parameterList[index] == null || parameterList[index].Count == 0)
                    {
                        validRequest = false;
                        break;
                    }
                    else
                    {
                        temp = new SqlCommand(queries[index]);
                        foreach (string key in parameterList[index].Keys)
                        {
                            temp.Parameters.Add(key, parameterList[index][key]);
                        }

                        commands.Add(temp);
                    }
                }

                if (!validRequest)
                    return false;
                else
                    return ExecuteNonQueryWithMultiple(commands);
            }
        }
        public int ExecuteNonQuery(SqlCommand command)
        {
            int rowsAffected = 0;

            if (command != null)
            {
                var connection = new SqlConnection();
                SqlTransaction ot = null;
                try
                {
                    connection.ConnectionString = GetConnectionString();
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Open();
                    ot = connection.BeginTransaction();
                    command.Connection = connection;
                    command.Transaction = ot;

                    rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected < 0)
                    {
                        ot.Rollback();
                        rowsAffected = 0;
                    }
                    else
                    {
                        ot.Commit();
                    }
                }
                catch (SqlException oex)
                {
                    ot.Rollback();
                    LogMessageToFile(oex.Message, oex.StackTrace);
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                    rowsAffected = 0;
                }
                finally
                {
                    command.Dispose();
                    if (connection != null)
                        connection.Close();
                }
            }
            return rowsAffected;
        }
        public bool ExecuteNonQueryWithMultiple(List<SqlCommand> commands)
        {
            bool operationStatus = false;
            int rowsAffected = 0;
            bool transactionCompleted = true;

            if (commands != null && commands.Count > 0)
            {
                var connection = new SqlConnection();
                SqlTransaction transaction = null;
                try
                {
                    connection.ConnectionString = GetConnectionString();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    transaction = connection.BeginTransaction();
                    foreach (var command in commands)
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                    }

                    foreach (var command in commands)
                    {
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected < 0)
                        {
                            transactionCompleted = false;
                            break;
                        }
                    }

                    if (!transactionCompleted)
                    {
                        transaction.Rollback();
                        operationStatus = false;
                    }
                    else
                    {
                        transaction.Commit();
                        operationStatus = true;
                    }
                }
                catch (SqlException oex)
                {
                    transaction.Rollback();
                    operationStatus = false;
                    LogMessageToFile(oex.Message, oex.StackTrace);
                }
                finally
                {
                    foreach (var command in commands)
                    {
                        command.Dispose();
                    }

                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return operationStatus;
        }

        #endregion

        // Utility Logging
        public void LogMessageToFile(string errorMessage, string errorDetails)
        {
            string directory = "~/Logs";
            if ((!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(directory))))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(directory));
            }

            string file = "~/Logs/" + "Exception_Log" + "_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
            if ((!File.Exists(System.Web.HttpContext.Current.Server.MapPath(file))))
            {
                File.Create(System.Web.HttpContext.Current.Server.MapPath(file)).Close();
            }

            using (StreamWriter w = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(file)))
            {
                w.WriteLine("Log Entry : ");
                w.WriteLine("Team Member: " + "SAKSHI VERMA");
                w.WriteLine("Error Time: " + DateTime.Now);
                w.WriteLine("Error Message: " + errorMessage);
                w.WriteLine("Error Details: " + errorDetails);
                w.WriteLine("_____________________________________________________________________");
                w.Flush();
                w.Close();
            }

        }
    }
}
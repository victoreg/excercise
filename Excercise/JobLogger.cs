using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Excercise
{
    public class JobLogger
    {
        private static bool logToFile;
        private static bool logToConsole;
        private static bool logToDatabase;
        private static bool logMessage;
        private static bool logWarning;
        private static bool logError;
        //private bool _initialized;
        private static string datetimeNow = DateTime.Now.ToString("ddMMyyyy");

        public JobLogger(bool _logToFile, bool _logToConsole, bool _logToDatabase, bool _logMessage, bool _logWarning, bool _logError)
        {
            logError = _logError;
            logMessage = _logMessage;
            logWarning = _logWarning;
            logToDatabase = _logToDatabase;
            logToFile = _logToFile;
            logToConsole = _logToConsole;

            if (!logToConsole && !logToFile && !logToDatabase)
            {
                throw new ArgumentException("Invalid configuration, please enter almost one log method.");
            }
        }

        public void LogMessage(string message, bool messageE, bool warning, bool error)
        {
            message.Trim();
            if (message == null || message.Length == 0)
            {
                return;
            }
            if ((!logError && !logMessage && !logWarning) || (!messageE && !warning && !error))
            {
                throw new ArgumentException("Error or Warning or Message must be specified");
            }

            if (logToConsole)
            {
                LogOnConsole(message, messageE, warning, error);
            }
            if (logToFile)
            {
                LogToTextFile(message, messageE, warning, error);
            }
            if (logToDatabase)
            {
                LogToDB(message, messageE, warning, error);
            }
        }

        private static void LogToDB(string message, bool messageE, bool warning, bool error)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString()))
            {
                try
                {
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Open();
                    }

                    int t = -1;
                    if (messageE && logMessage)
                    {
                        t = 1;
                    }
                    if (error && logError)
                    {
                        t = 2;
                    }
                    if (warning && logWarning)
                    {
                        t = 3;
                    }
                    string sql = String.Format("Insert into Log Values('{0}', {1})", message, t.ToString());
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Log to database failed. An error occurs: " + ex.Message);
                }
                finally
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        private static async void LogToTextFile(string message, bool messageE, bool warning, bool error)
        {
            string l = string.Empty;
            string txtfilePath = System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"].ToString() + "LogFile" + datetimeNow + ".txt";
            message = String.Format("{0} {1} {2}", DateTime.Now.ToString(), message, Environment.NewLine);
            try
            {
                if (!System.IO.File.Exists(txtfilePath))
                {
                    using (StreamWriter sw = File.CreateText(txtfilePath))
                    {
                        await sw.WriteAsync(message);
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(txtfilePath, true))
                    {
                        await sw.WriteLineAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Log to text file failed. An error occurs: " + ex.Message);
            }
        }

        private static void LogOnConsole(string message, bool messageE, bool warning, bool error)
        {
            if (error && logError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (warning && logWarning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (messageE && logMessage)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(DateTime.Now.ToShortDateString() + " " + message);
            Console.ForegroundColor = ConsoleColor.Gray;
            //Console.ReadLine();
        }
    }
}
using System;
using System.Data.SqlClient;
using System.IO;
using DataManager;

namespace ApplicationInsights
{
    public static class Logger
    {

        private static object _obj = new object();
        public static string LogFilePath { private get; set; }
        public static DataAccess DataAccess { private get; set; }

        public static void RecordEntry(string fileEvent, string filePath)
        {
            lock (_obj)
            {
                var message = String.Format("file {0} has been {1}", filePath, fileEvent);
                var date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                var fullMessage = string.Format("{0} {1}", date, message);
                Console.WriteLine(fullMessage);
                if (!(LogFilePath == null))
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(fullMessage);
                        writer.Flush();
                    }
                if (!(DataAccess == null))
                {
                    DataAccess.ExecuteStoredProcedure("AddLogEntry", new SqlParameter("@datetime", date), new SqlParameter("@message", message));
                }
            }
        }
        public static void RecordStatus(string status)
        {
            lock (_obj)
            {
                var message = String.Format("status: {0}", status);
                var date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                var fullMessage = string.Format("{0} {1}", date, message);
                Console.WriteLine(fullMessage);
                if (!(LogFilePath == null))
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(fullMessage);
                        writer.Flush();
                    }
                if (!(DataAccess == null))
                {
                    DataAccess.ExecuteStoredProcedure("AddLogEntry", new SqlParameter("@datetime", date), new SqlParameter("@message", message));
                }
            }
        }
        public static void RecordError(string errorMessage, object sender)
        {
            lock (_obj)
            {
                var message = String.Format("error: {0} thrown exception:\n{1}", sender.ToString(), errorMessage);
                var date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                var fullMessage = string.Format("{0} {1}", date, message);
                Console.WriteLine(fullMessage);
                if (!(LogFilePath == null))
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(fullMessage);
                        writer.Flush();
                    }
                if (!(DataAccess == null))
                {
                    DataAccess.ExecuteStoredProcedure("AddLogEntry", new SqlParameter("@datetime", date), new SqlParameter("@message", message));
                }
            }
        }
    }
}

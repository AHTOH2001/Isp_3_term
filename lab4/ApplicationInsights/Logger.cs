using System;
using System.IO;

namespace ApplicationInsights
{
    public static class Logger
    {

        private static object _obj = new object();
        public static string LogFilePath;

        public static void RecordEntry(string fileEvent, string filePath)
        {
            lock (_obj)
            {
                var message = String.Format("{0} file {1} has been {2}",
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), filePath, fileEvent);
                Console.WriteLine(message);
                if (!(LogFilePath == null))
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
            }
        }
        public static void RecordStatus(string status)
        {
            lock (_obj)
            {
                var message = String.Format("{0} status: {1}",
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), status);
                Console.WriteLine(message);
                if (!(LogFilePath == null))
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
            }
        }
        public static void RecordError(string errorMessage, object sender)
        {
            lock (_obj)
            {
                var message = String.Format("{0} error: {1} thrown exception:\n{2}",
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), sender.ToString(), errorMessage);
                Console.WriteLine(message);
                if (!(LogFilePath == null))
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
            }
        }
    }
}

using System;
using System.IO;

namespace lab2
{
    public static class Logger
    {

        static object obj = new object();
        public static string logFilePath;

        public static void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    var message = String.Format("{0} file {1} has been {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent);

                    writer.WriteLine(message);
                    writer.Flush();
                    Console.WriteLine(message);
                }
            }
        }
        public static void RecordStatus(string Status)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    var message = String.Format("{0} status: {1}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), Status);

                    writer.WriteLine(message);
                    writer.Flush();
                    Console.WriteLine(message);
                }
            }
        }
    }
}

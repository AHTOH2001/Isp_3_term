using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace lab2
{
    class Logger
    {

        static object obj = new object();
        public static string LogFilePath;
        public Logger(string logFilePath)
        {
            LogFilePath = logFilePath;
        }

        static public void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                    Console.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                }
            }
        }
    }
}

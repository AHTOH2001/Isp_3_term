﻿using System;
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
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {                                        
                    var message = String.Format("{0} file {1} has been {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent);

                    writer.WriteLine(message);
                    writer.Flush();
                    Console.WriteLine(message);
                }
            }
        }
        public static void RecordStatus(string status)
        {            
            lock (_obj)
            {
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    var message = String.Format("{0} status: {1}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), status);

                    writer.WriteLine(message);
                    writer.Flush();
                    Console.WriteLine(message);
                }
            }
        }
    }
}

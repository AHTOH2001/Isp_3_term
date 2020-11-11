using lab2;
using ServiceLibraries_lab3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace lab3_withoutServices
{
    class Program
    {
        static Watcher watcher;
        static void Main(string[] args)
        {
            //watcher = new Watcher();
            //Thread watcherThread = new Thread(new ThreadStart(watcher.Start));
            //watcherThread.Start();     
            
            SystemConfiguration systemConfiguration = new SystemConfiguration(xmlFileName: "config.xml");

            var archiveOptions = new ArchiveOptions();
            var allConfigurations = new AllConfigurations();
            var e = systemConfiguration.GetConfigurationClass(allConfigurations);
            {
                Console.WriteLine(e.Name);
                Console.WriteLine("fields count :  {0}", e.GetFields().Length);
                foreach (var e1 in e.GetFields())
                {
                    Console.WriteLine($"Field name: {e1.Name}");
                    Type val = e1.GetValue(null) as Type;                    
                    Console.WriteLine("fields count : {0}", val.GetFields().Length);
                }
            }

        }
    }
}

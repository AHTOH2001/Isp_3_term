using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager;
using ConfigurationManager;
using ApplicationInsights;
using System.Threading;

namespace DataManager
{
    public class DataManagerClass
    {
        Thread watcherThread;
        Thread serviceThread;
        Service service;
        Watcher watcher;
        public void Start()
        {
            try
            {
                SystemConfiguration systemConfiguration = new SystemConfiguration();
                watcher = new Watcher(systemConfiguration);
                watcherThread = new Thread(new ThreadStart(watcher.Start));
                watcherThread.Start();
                service = new Service(systemConfiguration);
                serviceThread = new Thread(new ThreadStart(service.Start));
                serviceThread.Start();
            }
            catch (Exception e)
            {
                Logger.RecordError(e.Message, this);
            }


        }
        public void Stop()
        {
            watcher.Stop();
            service.Stop();
            Thread.Sleep(1000);
        }
    }
}

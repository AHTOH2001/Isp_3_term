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
        public void Start()
        {
            try
            {
                ConfigurationManager.SystemConfiguration systemConfiguration = new SystemConfiguration();
                FileManager.Watcher watcher = new Watcher(systemConfiguration);
                watcherThread = new Thread(new ThreadStart(watcher.Start));
                watcherThread.Start();
                Service service = new Service(systemConfiguration);
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

        }
    }
}

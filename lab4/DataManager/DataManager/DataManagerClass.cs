using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager;
using ConfigurationManager;


namespace DataManager
{
    public class DataManagerClass
    {
        public void Start()
        {
            ConfigurationManager.SystemConfiguration systemConfiguration = new SystemConfiguration();
            Service service = new Service(systemConfiguration);
            service.Start();            
            FileManager.Watcher watcher = new Watcher(systemConfiguration);
            watcher.Start();
                
        }
    }
}

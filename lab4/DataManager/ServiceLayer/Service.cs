using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationManager;

namespace DataManager
{
    public class Service
    {
        private SystemConfiguration _systemConfiguration;
        public Service(SystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
        }
        public void Start()//Should rename
        {
            DataAccess dataAccess = new DataAccess(_systemConfiguration);
            Console.WriteLine(dataAccess.procWithStoredProcedure1("GetDepartments"));
        }
        
    }
}

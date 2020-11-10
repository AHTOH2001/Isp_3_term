using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibraries_lab3
{
    public class Options
    {
        Dictionary<string, int> intValue;
        Dictionary<string, bool> boolValue;
        Dictionary<string, string> stringValue;
        public object this[string key]
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }
    }
}

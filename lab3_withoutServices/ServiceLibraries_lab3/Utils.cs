using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibraries_lab3
{
    public static class Utils
    {
        public static string GetOptionPseudonim(Type option)
        {            
            foreach (ConfigurationPseudonymAttribute attribute in option.GetCustomAttributes(false))
            {
                return attribute.Pseudonym;
            }
            return option.Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibraries_lab3
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class JsonFileNameAttribute : System.Attribute
    {
        public string Name { get; set; }
        public JsonFileNameAttribute(string name)
        {
            Name = Path.ChangeExtension(name, "json");            
        }
    }
}

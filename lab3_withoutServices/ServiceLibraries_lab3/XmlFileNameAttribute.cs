using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibraries_lab3
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class XmlFileNameAttribute : System.Attribute
    {
        public string Name { get; set; }
        public XmlFileNameAttribute(string name)
        {
            Name = Path.ChangeExtension(name, "xml");
        }
    }
}

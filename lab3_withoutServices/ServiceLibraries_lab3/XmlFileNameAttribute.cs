using System;
using System.IO;

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

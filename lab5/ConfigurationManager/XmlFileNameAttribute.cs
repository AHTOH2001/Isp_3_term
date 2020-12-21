using System;
using System.IO;

namespace ConfigurationManager
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

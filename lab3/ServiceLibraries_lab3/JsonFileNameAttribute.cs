using System;
using System.IO;

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

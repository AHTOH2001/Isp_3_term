using System;

namespace ServiceLibraries_lab3
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationPseudonymAttribute : System.Attribute
    {
        public string Pseudonym { get; set; }
        public ConfigurationPseudonymAttribute(string pseudonym)
        {
            Pseudonym = pseudonym;
        }
    }
}

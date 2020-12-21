using System;

namespace Models
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

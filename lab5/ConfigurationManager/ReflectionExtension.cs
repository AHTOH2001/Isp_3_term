using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationManager
{
    public static class ReflectionExtension
    {
        public static T GetOption<T>(this Type source, string name)
        {
            try
            {
                return (T)source.GetField(name).GetValue(null);
            }
            catch 
            {
                throw new KeyNotFoundException($"Option {name} was not found in {source.Name}");
            }
        }
    }
}

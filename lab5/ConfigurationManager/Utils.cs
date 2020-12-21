using System;

namespace ConfigurationManager
{
    public class Utils
    {
        public T GetAttribute<T>(object sourceClass) where T : Attribute
        {
            foreach (var member in sourceClass.GetType().GetMembers())
            {
                foreach (var attribute in member.GetCustomAttributes(false))
                    if (attribute is T)
                    {
                        return attribute as T;
                    }
            }
            foreach (var attribute in sourceClass.GetType().GetCustomAttributes(false))
                if (attribute is T)
                {
                    return attribute as T;
                }
            return null;
        }
        public Type FigureOutType(string inputString)
        {
            if (bool.TryParse(inputString, out _))
            {
                return typeof(bool);
            }
            else if (int.TryParse(inputString, out _))
            {
                return typeof(int);
            }
            else
            {
                return typeof(string);
            }
        }
    }
}

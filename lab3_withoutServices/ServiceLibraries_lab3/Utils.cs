using System;

namespace ServiceLibraries_lab3
{
    public static class Utils
    {
        public static T GetAttribute<T>(object sourceClass) where T : Attribute
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
    }
}

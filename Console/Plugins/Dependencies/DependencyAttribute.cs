using System;

namespace Console.Plugins.Dependencies
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DependencyAttribute : Attribute
    {
        public string Name { get; }

        public DependencyAttribute(string name)
        {
            Name = name;
        }
    }
}
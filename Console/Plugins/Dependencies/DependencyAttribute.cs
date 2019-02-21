using System;

namespace Console.Plugins.Dependencies
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DependencyAttribute : Attribute
    {
        internal string Name { get; }

        public DependencyAttribute(string name)
        {
            Name = name;
        }
    }
}
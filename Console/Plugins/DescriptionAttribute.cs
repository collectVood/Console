using System;

namespace Console.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        internal string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
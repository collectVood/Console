using System;

namespace Console.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string Description)
        {
            this.Description = Description;
        }
    }
}
using System;

namespace Console.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        internal string Name { get; }
        internal string Prefix { get; }

        public CommandAttribute(string name, string prefix = null)
        {
            Name = name;
            Prefix = prefix;
        }
    }
}
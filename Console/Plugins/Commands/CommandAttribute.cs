using System;

namespace Console.Plugins.Commands
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        internal string Name { get; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
using System;

namespace Console.Plugins.Commands
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
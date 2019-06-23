using System;

namespace Console.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableAttribute : Attribute
    {
        internal string Name { get; }

        public VariableAttribute(string name)
        {
            Name = name;
        }
    }
}
using System;

namespace Console.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableAttribute : Attribute
    {
        internal string Name { get; }
        internal string Prefix { get; }

        public VariableAttribute(string name, string prefix = null)
        {
            Name = name;
            Prefix = prefix;
        }
    }
}
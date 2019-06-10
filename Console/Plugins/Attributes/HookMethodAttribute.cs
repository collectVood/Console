using System;

namespace Console.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HookMethodAttribute : Attribute
    {
        internal string Name;

        public HookMethodAttribute(string name)
        {
            Name = name;
        }
    }
}
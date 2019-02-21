using System;

namespace Console.Plugins.Hooks
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
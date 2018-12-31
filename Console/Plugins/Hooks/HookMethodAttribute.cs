using System;

namespace Console.Plugins.Hooks
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HookMethodAttribute : Attribute
    {
        public string Name;

        public HookMethodAttribute(string name)
        {
            Name = name;
        }
    }
}
using System;

namespace Console.Plugins
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
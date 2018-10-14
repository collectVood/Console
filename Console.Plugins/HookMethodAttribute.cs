using System;

namespace Console.Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HookMethodAttribute : Attribute
    {
        public string Name;

        public HookMethodAttribute(string name)
        {
            Name = name;
        }
    }
}
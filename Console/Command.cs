using System;
using System.Reflection;

namespace Console.Plugins
{
    public class Command
    {
        public Plugin Owner { get; internal set; }
        public MethodInfo Method { get; internal set; }
        
        public string Name { get; internal set; }
        public string FullName { get; internal set; }

        public Command(Plugin plugin, string name, MethodInfo method)
        {
            Owner = plugin;
            Method = method;
            
            Name = name.ToLower();
            FullName = $"{plugin.Name}.{name}".ToLower();
        }

        public void Call(params object[] args)
        {
            try
            {
                Method?.Invoke(Owner, args);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}
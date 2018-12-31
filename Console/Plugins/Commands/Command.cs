using System;
using System.Reflection;

namespace Console.Plugins.Commands
{
    public class Command
    {
        public Plugin Owner { get; }
        public MethodInfo Method { get; }
        
        public string Name { get; }
        public string FullName { get; }

        public Command(Plugin plugin, string name, MethodInfo method)
        {
            Owner = plugin;
            Method = method;
            
            Name = name.ToLower();
            FullName = $"{plugin.Name}.{name}".ToLower();
        }

        public void Call(string command)
        {
            if (!CanUseCommand(command))
                return;
            
            try
            {
                Method?.Invoke(Owner, null);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        internal bool CanUseCommand(string parameters) => parameters.Length == 0;
    }
}
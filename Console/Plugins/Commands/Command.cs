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
            
            Name = name;
            FullName = $"{plugin.Name}.{name}".ToLower();
        }

        public void Execute(string[] arguments)
        {
            try
            {
                Method?.Invoke(Owner, null);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public static bool HasMatchingSignature(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]);
        }
    }
}
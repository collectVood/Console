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

            name = name.ToLower();
            
            Name = name;
            FullName = $"{plugin.Name}.{name}".ToLower();
        }

        /// <summary>
        /// Execute the command with specified CommandArgument
        /// </summary>
        /// <param name="arg">Command argument</param>
        public void Execute(CommandArgument arg)
        {
            try
            {
                Method?.Invoke(Owner, new object[] {arg});
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        /// <summary>
        /// Returns true if the method has a matching signature for command
        /// </summary>
        /// <param name="method">Method Info instance for the needed method</param>
        /// <returns>True if the method has a matching signature for command</returns>
        public static bool HasMatchingSignature(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(CommandArgument);
        }
    }
}
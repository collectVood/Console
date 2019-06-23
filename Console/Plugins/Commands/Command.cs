using System;
using System.Reflection;
using System.Text;

namespace Console.Plugins.Commands
{
    public class Command
    {
        public Plugin Owner { get; }
        public Action<CommandArgument> CommandAction { get; }
        public MemberInfo MemberInfo { get; }
        
        public bool IsVariable { get; }
        
        public string Name { get; }
        public string FullName { get; }

        internal Command(Plugin plugin, string name, Action<CommandArgument> action)
            : this(plugin, name)
        {
            CommandAction = action;
            IsVariable = false;
        }

        internal Command(Plugin plugin, string name, MemberInfo memberInfo)
            : this(plugin, name)
        {
            MemberInfo = memberInfo;
            IsVariable = true;
        }

        private Command(Plugin plugin, string name)
        {
            Owner = plugin;
            Name = name.ToLower();
            FullName = $"{plugin.Name}.{name}".ToLower();
        }

        /// <summary>
        /// Execute the command with specified CommandArgument
        /// </summary>
        /// <param name="arg">Command argument</param>
        public bool Execute(CommandArgument arg)
        {
            try
            {
                if (IsVariable)
                {
                    if (arg.HasArgs())
                    {
                        var result = TrySetVariable(arg);
                        PrintVariable(arg, result ? GetVariable() : null);
                        return result;
                    }

                    var value = GetVariable();
                    PrintVariable(arg, value);
                    return false;
                }

                if (CommandAction == null)
                    return false;
                    
                CommandAction?.Invoke(arg);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return false;
        }

        private void PrintVariable(CommandArgument arg, string value)
        {
            if (value != null)
            {
                arg.Reply($"{FullName}: {value}");
                return;
            }
                    
            arg.Reply("Could not parse the value");
        }

        /// <summary>
        /// Set a field/property value
        /// </summary>
        /// <param name="arg">Command argument</param>
        /// <returns>True if successful</returns>
        private bool TrySetVariable(CommandArgument arg)
        {
            var field = MemberInfo as FieldInfo;
            if (field != null)
            {
                var type = field.FieldType;
                var success = Converter.TryConvert(arg.Arguments, type, out var result);

                if (!success)
                    return false;
                
                field.SetValue(Owner, result);
                return true;
            }

            var property = MemberInfo as PropertyInfo;
            // ReSharper disable once InvertIf
            if (property != null)
            {
                var type = property.PropertyType;
                var success = Converter.TryConvert(arg.Arguments, type, out var result);

                if (!success)
                    return false;
                
                property.SetValue(Owner, result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get a field/property value
        /// </summary>
        /// <returns>True if successful</returns>
        private string GetVariable()
        {
            var field = MemberInfo as FieldInfo;
            if (field != null)
            {
                var value = field.GetValue(Owner);
                return value.ToString();
            }

            var property = MemberInfo as PropertyInfo;
            // ReSharper disable once InvertIf
            if (property != null)
            {
                var value = property.GetValue(Owner);
                return value.ToString();
            }
            
            return null;
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
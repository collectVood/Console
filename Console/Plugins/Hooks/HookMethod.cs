using System;
using System.Reflection;

namespace Console.Plugins.Hooks
{
    public class HookMethod
    {
        public string Name { get; }
        public MethodInfo Method { get; }
        public Plugin Owner { get; }
        public ParameterInfo[] GetArguments() => Method.GetParameters();

        public bool IsCoreHook => Name.StartsWith("I");

        public HookMethod(Plugin plugin, string name, MethodInfo method)
        {
            Method = method;
            Name = name;
            Owner = plugin;
        }

        /// <summary>
        /// Format arguments to be the same length as hook requires
        /// </summary>
        /// <param name="args1">Input parameters array</param>
        /// <returns>Formatted parameters array</returns>
        internal object[] FormatArguments(object[] args1)
        {
            var args2 = GetArguments();

            var length1 = args1?.Length ?? 0;
            var length2 = args2?.Length ?? 0;
            
            if (length1 < length2)
                return null;
            
            Array.Resize(ref args1, length2);
            return args1;
        }

        /// <summary>
        /// Check if arguments have same signature with the method
        /// </summary>
        /// <param name="args1">Input arguments</param>
        /// <returns>True if arguments are valid for this hook method</returns>
        internal bool HasMatchingSignature(object[] args1)
        {
            var args2 = GetArguments();

            var length1 = args1?.Length ?? 0;
            var length2 = args2?.Length ?? 0;
            
            var toReturn = true;

            if (length1 == 0 && length2 == 0)
                return true;

            for (var i = 0; i < length1; i++)
            {
                var arg1 = args1?[i];
                var arg2 = args2?.Length < i + 1 ? null : args2?[i];

                if (arg1?.GetType() != arg2?.ParameterType && arg1 != null && arg2 != null)
                    toReturn = false;
            }

            return toReturn;
        }
        
        /// <summary>
        /// Check if arguments could be used for method
        /// </summary>
        /// <param name="plugin">Plugin instance</param>
        /// <param name="args1">Input arguments</param>
        /// <returns>True if input arguments are valid for the specified plugin-caller and current hook</returns>
        internal bool CanUseHook(Plugin plugin, object[] args1)
        {
            if (!plugin.IsCorePlugin && IsCoreHook)
                return false;
            
            var args2 = GetArguments();
            var length1 = args1?.Length ?? 0;
            var length2 = args2?.Length ?? 0;
            if (args1 == null && args2 == null || length1 == 0 && length2 == 0)
                return true;

            var args = new object[length2];

            if (length2 > length1)
            {
                for (var i = 0; i < length2; i++)
                {
                    var param = args2?[i];
                    if (param?.DefaultValue != null)
                        args[i] = param.DefaultValue;
                    else if (param != null)
                        args[i] = Activator.CreateInstance(param.ParameterType);
                    else
                        args[i] = null;
                }
            }
            else
                args = args1;

            return HasMatchingSignature(args);
        }
    }
}
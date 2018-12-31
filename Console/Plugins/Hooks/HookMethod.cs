using System;
using System.Reflection;

namespace Console.Plugins.Hooks
{
    public class HookMethod
    {
        public string Name { get; }
        public MethodInfo Method { get; }
        public Plugin Owner { get; }
        public ParameterInfo[] Parameters() => Method.GetParameters();

        public bool IsCoreHook => Name.StartsWith("I");

        public HookMethod(Plugin plugin, string name, MethodInfo method)
        {
            Method = method;
            Name = name;
            Owner = plugin;
        }

        internal object[] FormatArguments(object[] params1)
        {
            var params2 = Parameters();

            var length1 = params1?.Length ?? 0;
            var length2 = params2?.Length ?? 0;
            
            if (length1 < length2)
                return null;
            
            Array.Resize(ref params1, length2);
            return params1;
        }

        /// <summary>
        /// Check if params have same signature with the method
        /// </summary>
        /// <param name="params1"></param>
        /// <returns></returns>
        internal bool HasMatchingSignature(object[] params1)
        {
            var params2 = Parameters();

            var length1 = params1?.Length ?? 0;
            var length2 = params2?.Length ?? 0;
            
            var toReturn = true;

            if (length1 == 0 && length2 == 0)
                return true;

            for (var i = 0; i < length1; i++)
            {
                var param1 = params1?[i];
                var param2 = params2?.Length < i + 1 ? null : params2?[i];

                if (param1?.GetType() != param2?.ParameterType && param1 != null && param2 != null)
                    toReturn = false;
            }

            return toReturn;
        }
        
        /// <summary>
        /// Check if parameters could be used for method
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="params1">Parameters</param>
        internal bool CanUseHook(Plugin plugin, object[] params1)
        {
            if (!plugin.IsCorePlugin && IsCoreHook)
                return false;
            
            var params2 = Parameters();
            var length1 = params1?.Length ?? 0;
            var length2 = params2?.Length ?? 0;
            if (params1 == null && params2 == null || length1 == 0 && length2 == 0)
                return true;

            var arr = new object[length2];

            if (length2 > length1)
            {
                for (var i = 0; i < length2; i++)
                {
                    var param = params2?[i];
                    if (param?.DefaultValue != null)
                        arr[i] = param.DefaultValue;
                    else if (param != null)
                        arr[i] = Activator.CreateInstance(param.ParameterType);
                    else
                        arr[i] = null;
                }
            }
            else
                arr = params1;

            return HasMatchingSignature(arr);
        }
    }
}
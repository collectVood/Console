using System.Reflection;

namespace Console.Plugins
{
    public class HookMethod
    {
        public string Name { get; }
        public MethodInfo Method { get; }
        public Plugin Owner { get; }
        public ParameterInfo[] Parameters() => Method.GetParameters();

        public HookMethod(Plugin plugin, string name, MethodInfo method)
        {
            Method = method;
            Name = name;
            Owner = plugin;
        }

        /// <summary>
        /// Check if params have same signature with the method
        /// </summary>
        /// <param name="params1"></param>
        /// <returns></returns>
        public bool HasMatchingSignature(object[] params1)
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
                var param2 = params2?[i];

                if (param1?.GetType() != param2?.ParameterType)
                    toReturn = false;
            }

            return toReturn;
        }
    }
}
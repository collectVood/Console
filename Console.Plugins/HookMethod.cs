using System.Reflection;

namespace Console.Plugins
{
    public class HookMethod
    {
        public string Name;
        public MethodInfo Method;
        public ParameterInfo[] Parameters() => Method.GetParameters();

        public HookMethod(string name, MethodInfo method)
        {
            Method = method;
            Name = name;
        }
    }
}
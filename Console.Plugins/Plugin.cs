using System;
using System.Collections.Generic;
using System.Reflection;

namespace Console.Plugins
{
    public class Plugin
    {
        #region Variables
        
        protected Dictionary<string, List<HookMethod>> Hooks = new Dictionary<string, List<HookMethod>>();
        
        #endregion
        
        public Plugin()
        {
            var type = GetType();
            var typeList = new List<Type>()
            {
                type
            };
            while (type != typeof(Plugin))
                typeList.Add(type = type?.BaseType);

            var typeCount = typeList.Count;
            for (var i1 = 0; i1 < typeCount; i1++)
            {
                type = typeList[i1];
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var methodsCount = methods.Length;
                for (var i2 = 0; i2 < methodsCount; i2++)
                {
                    var method = methods[i2];
                    if (method.GetCustomAttributes<HookMethodAttribute>(true) is List<HookMethodAttribute> attributes &&
                        attributes.Count > 0)
                    {
                        AddHookMethod(attributes[0]?.Name, method);
                    }
                }
            }
        }
        
        #region Hooks

        public void AddHookMethod(string name, MethodInfo method)
        {
            if (!Hooks.TryGetValue(name, out var hookMethods))
            {
                hookMethods = new List<HookMethod>();
                Hooks[name] = hookMethods;
            }
            
            hookMethods.Add(new HookMethod(name, method));
        }

        public object OnCallHook(string name, object[] args)
        {
            object toReturn = null;
            var flag = false;
            var hooks = FindHooks(name, args);
            
            return toReturn;
        }

        public List<HookMethod> FindHooks(string name, object[] args)
        {

            var toReturn = new List<HookMethod>();
            if (!Hooks.TryGetValue(name, out var methods))
                return toReturn;
            
            var methodsCount = methods.Count;
            for (var i = 0; i < methodsCount; i++)
            {
                var method = methods[i];
                if (args == null || args.Length == 0 && method.Parameters().Length == 0)
                {
                    toReturn.Add(method);
                    continue;
                }
                
                
            }

            return toReturn;
        }
        
        #endregion
    }
}
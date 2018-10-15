using System;
using System.Collections.Generic;
using System.Reflection;

namespace Console.Plugins
{
    public class Plugin
    {
        #region Variables

        public string Name { get; }
        public string Filename { get; }
        public string Description { get; }
        public string Author { get; }
        public 
        
        public bool IsCorePlugin { get; }
        
        protected internal Dictionary<string, List<HookMethod>> Hooks = Pool<Dictionary<string, List<HookMethod>>>.Get();
        
        #endregion
        
        public Plugin()
        {
            var type = GetType();
            var typeList = Pool<List<Type>>.Get();
            typeList.Add(type);
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

        /// <summary>
        /// Adds a hook method to plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="method"></param>
        public void AddHookMethod(string name, MethodInfo method)
        {
            if (!Hooks.TryGetValue(name, out var hookMethods))
            {
                hookMethods = Pool<List<HookMethod>>.Get();
                Hooks[name] = hookMethods;
            }
            
            hookMethods.Add(new HookMethod(this, name, method));
        }

        /// <summary>
        /// Calls hooks if they exist
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object OnCallHook(string name, object[] args)
        {
            object toReturn = null;
            var flag = false;
            var hooks = FindHooks(name, args);
            var hooksCount = hooks.Count;
            for (var i = 0; i < hooksCount; i++)
            {
                var method = hooks[i];

                try
                {
                    toReturn = method.Method.Invoke(this, args);
                }
                catch (Exception e)
                {
                    
                    throw e;
                }
            }
            
            return toReturn;
        }

        /// <summary>
        /// Find all matching hooks for arguments
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<HookMethod> FindHooks(string name, object[] args)
        {
            var toReturn = new List<HookMethod>();
            if (!Hooks.TryGetValue(name, out var methods))
                return toReturn;
            
            var methodsCount = methods.Count;
            for (var i = 0; i < methodsCount; i++)
            {
                var method = methods[i];
                var length1 = args?.Length ?? 0;
                var length2 = method.Parameters()?.Length ?? 0;
                
                if (length1 == 0 && length2 == 0 || CanUse(args, method))
                    toReturn.Add(method);
            }

            return toReturn;
        }

        /// <summary>
        /// Check if parameters could be used for method
        /// </summary>
        /// <param name="params1">Parameters</param>
        /// <param name="method"></param>
        public static bool CanUse(object[] params1, HookMethod method)
        {
            var params2 = method.Parameters();

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

            return method.HasMatchingSignature(arr);
        }

        #endregion
    }
}
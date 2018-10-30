using System;
using System.Collections.Generic;
using System.Reflection;
// ReSharper disable MemberCanBePrivate.Global

namespace Console.Plugins
{
    public class Plugin
    {
        #region Variables

        public string Name { get; internal set; }
        public string Title { get; internal set; }
        public string Filename { get; internal set; }
        public string Path { get; internal set; }
        public string Description { get; internal set; }
        public string Author { get; internal set; }
        public Version Version { get; internal set; }
        
        public bool IsCorePlugin { get; internal set; }
        
        protected internal Dictionary<string, HookMethod> Hooks = PoolNew<Dictionary<string, HookMethod>>.Get();
        protected internal Dictionary<string, Command> Commands = PoolNew<Dictionary<string, Command>>.Get();
        
        #endregion

        internal static void CreatePlugin(Type type, string path, bool corePlugin)
        {
            if (type == null || !(Activator.CreateInstance(type) is Plugin plugin))
            {
                Log.Error($"Failed to load plugin with path: {path}");
                return;
            }

            plugin.Path = path;
            plugin.Filename = string.IsNullOrEmpty(path) ? path : System.IO.Path.GetFileName(path);
            plugin.IsCorePlugin = corePlugin;
            
            Interface.Plugins.Add(plugin);
            Log.Info($"Loaded plugin {plugin.Title} from {plugin.Author} v{plugin.Version}");
        }

        protected Plugin()
        {
            var type = GetType();

            Name = type.Name;
            Title = type.Name;
            Author = "Unknown";
            Version = new Version(1, 0, 0);

            var info = type.GetCustomAttribute<InfoAttribute>();
            if (info != null)
            {
                Title = info.Title;
                Author = info.Author;
                Version = info.Version;
            }

            var description = type.GetCustomAttribute<DescriptionAttribute>();
            if (description != null)
            {
                Description = description.Description;
            }
            
            var typeList = PoolNew<List<Type>>.Get();
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
                    if (method.GetCustomAttribute<HookMethodAttribute>(false) is HookMethodAttribute attribute1)
                    {
                        AddHookMethod(attribute1.Name, method);
                    }

                    if (method.GetCustomAttribute<CommandAttribute>(false) is CommandAttribute attribute2)
                    {
                        AddCommand(attribute2.Name, method);
                    }
                }
            }
        }
        
        #region Hooks

        /// <summary>
        /// Calls a hook
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallHook(string name, params object[] args)
        {
            try
            {
                return OnCallHook(name, args);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to call hook '{name}' on '{Name}' v{Version}");
                Log.Exception(e);
                return null;
            }
        }

        /// <summary>
        /// Calls a hook
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Call(string name, params object[] args) => CallHook(name, args);

        public T Call<T>(string name, params object[] args)
        {
            var result = Call(name, args);
            return result == null ? default(T) : (T) Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Adds a hook method to plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="method"></param>
        public void AddHookMethod(string name, MethodInfo method)
        {
            if (Hooks.ContainsKey(name))
            {
                Log.Warning($"Plugin {Title} tried to register existing hook");
                return;
            }
            
            var hook = new HookMethod(this, name, method);
            if (!IsCorePlugin && hook.IsCoreHook)
                return;
            
            Hooks[name] = hook;
        }

        /// <summary>
        /// Calls hooks if they exist
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object OnCallHook(string name, object[] args)
        {
            var hooks = FindHook(name, args);
            try
            {
                return hooks?.Method?.Invoke(this, args);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return null;
        }

        /// <summary>
        /// Find all matching hooks for arguments
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public HookMethod FindHook(string name, object[] args) => !Hooks.TryGetValue(name, out var hook) || !CanUseHook(args, hook) ? null : hook;

        /// <summary>
        /// Check if parameters could be used for method
        /// </summary>
        /// <param name="params1">Parameters</param>
        /// <param name="method"></param>
        public static bool CanUseHook(object[] params1, HookMethod method)
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
        
        #region Commands

        public void AddCommand(string name, MethodInfo method)
        {
            if (Commands.ContainsKey(name))
            {
                Log.Warning($"Plugin {Title} is trying to register existing command");
                return;
            }

            Commands[name] = new Command(this, name, method);
        }

        #endregion
    }
}
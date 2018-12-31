using System;
using System.Collections.Generic;
using System.Reflection;
using Console.Plugins.Commands;
using Console.Plugins.Hooks;

// ReSharper disable MemberCanBePrivate.Global

namespace Console.Plugins
{
    public class Plugin
    {
        #region Variables

        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Filename { get; private set; }
        public string Path { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public Version Version { get; private set; }
        
        public bool IsLoaded { get; internal set; }
        public bool IsCorePlugin { get; private set; }
        
        protected internal Dictionary<string, HookMethod> Hooks = PoolNew<Dictionary<string, HookMethod>>.Get();
        protected internal Dictionary<string, Command> Commands = PoolNew<Dictionary<string, Command>>.Get();
        
        #endregion

        internal static bool CreatePlugin(Type type, string path)
        {
            try
            {
                if (type == null || Interface.FindPlugin(type.Name) != null)
                    return false;

                if (Activator.CreateInstance(type) is Plugin plugin)
                {
                    plugin.Initialize(path);
                    return true;
                }

                Log.Error($"Failed to load plugin with path: {path}");
                return false;

            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
        }

        public void Initialize(string path)
        {
            var type = GetType();

            Path = path;
            Filename = System.IO.Path.GetFileName(path);
            Name = type.Name;
            Title = type.Name;
            Author = "Unknown";
            Version = new Version(1, 0, 0);
            IsCorePlugin = string.IsNullOrEmpty(Path);

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
                    if (method.GetCustomAttribute<HookMethodAttribute>(false) is HookMethodAttribute hookMethodAttribute)
                    {
                        AddHookMethod(hookMethodAttribute.Name, method);
                    }

                    if (method.GetCustomAttribute<CommandAttribute>(false) is CommandAttribute commandAttribute)
                    {
                        AddCommand(commandAttribute.Name, method);
                    }
                }
            }
            
            Interface.Plugins.Add(this);
            Interface.Load(Name);
        }
        
        #region Hooks

        /// <summary>
        /// Calls a hook
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object CallHook(string name, params object[] args)
        {
            if (!IsLoaded)
                return null;
            
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
            
            Hooks[name] = new HookMethod(this, name, method);
        }

        /// <summary>
        /// Calls hooks if they exist
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object OnCallHook(string name, object[] args)
        {
            var hook = FindHook(name, args);
            try
            {
                return hook?.Method?.Invoke(this, hook.FormatArguments(args));
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
        public HookMethod FindHook(string name, object[] args) => !Hooks.TryGetValue(name, out var hook) || !hook.CanUseHook(this, args) ? null : hook;

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
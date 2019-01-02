using System;
using System.Collections.Generic;
using System.Reflection;
using Console.Plugins.Commands;
using Console.Plugins.Dependencies;
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
        
        protected internal Dictionary<string, HookMethod> Hooks { get; }= PoolNew<Dictionary<string, HookMethod>>.Get();
        protected internal Dictionary<string, Command> Commands { get; }= PoolNew<Dictionary<string, Command>>.Get();
        protected internal List<Dependency> Dependencies { get; }= PoolNew<List<Dependency>>.Get();
        
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
            for (var i = 0; i < typeCount; i++)
            {
                type = typeList[i];
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var methodsCount = methods.Length;
                for (var j = 0; j < methodsCount; j++)
                {
                    var method = methods[j];
                    if (method.GetCustomAttribute<HookMethodAttribute>(false) is HookMethodAttribute hookMethodAttribute)
                    {
                        AddHookMethod(hookMethodAttribute.Name, method);
                    }

                    if (method.GetCustomAttribute<CommandAttribute>(false) is CommandAttribute commandAttribute)
                    {
                        AddCommand(commandAttribute.Name, method);
                    }
                }

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fieldsCount = fields.Length;
                for (var j = 0; j < fieldsCount; j++)
                {
                    var field = fields[j];
                    if (field.GetCustomAttribute<DependencyAttribute>(false) is DependencyAttribute dependencyAttribute)
                    {
                        AddDependency(dependencyAttribute.Name, field);
                    }
                }
            }
            
            UpdateDependencies();
            
            Interface.Plugins.Add(this);
            Interface.Load(Name);
        }
        
        #region Dependencies

        /// <summary>
        /// Adds a dependency
        /// </summary>
        /// <param name="name">Dependency (plugin) name</param>
        /// <param name="field"></param>
        public void AddDependency(string name, FieldInfo field)
        {
            if (!Dependency.HasMatchingSignature(field))
            {
                Log.Warning($"Plugin {Title} tried to register dependency for an incorrect field");
                return;
            }
            
            Dependencies.Add(new Dependency(this, name, field));
        }

        /// <summary>
        /// Update all dependencies (find all dependencies)
        /// </summary>
        public void UpdateDependencies()
        {
            for (var i = 0; i < Dependencies.Count; i++)
            {
                Dependencies[i].Update();
            }
        }
        
        #endregion
        
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

        /// <summary>
        /// Adds a command to plugin
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="method"></param>
        public void AddCommand(string name, MethodInfo method)
        {
            name = name.ToLower();
            if (Commands.ContainsKey(name))
            {
                Log.Warning($"Plugin {Title} tried to register existing command");
                return;
            }

            if (!Command.HasMatchingSignature(method))
            {
                Log.Warning($"Plugin {Title} tried to register command with incorrect method arguments");
                return;
            }

            Commands[name] = new Command(this, name, method);
        }

        /// <summary>
        /// Find matching command
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Command FindCommand(string name)
        {
            name = name.ToLower();
            if (Commands.TryGetValue(name, out var command))
                return command;

            foreach (var kvp in Commands)
            {
                if (kvp.Value.FullName == name)
                    return kvp.Value;
            }

            return null;
        }

        #endregion
    }
}
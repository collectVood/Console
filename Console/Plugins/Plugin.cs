using System;
using System.Collections.Generic;
using System.Reflection;
using Console.Plugins.Attributes;
using Console.Plugins.Commands;
using Console.Plugins.Dependencies;
using Console.Plugins.Hooks;

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
        public bool IsUnloadable { get; private set; }
        
        protected internal Dictionary<string, HookMethod> Hooks { get; } = new Dictionary<string, HookMethod>();
        protected internal Dictionary<string, Command> Commands { get; } = new Dictionary<string, Command>();
        protected internal List<Dependency> Dependencies { get; } = new List<Dependency>();
        
        #endregion

        /// <summary>
        /// Create a plugin instance with the specified path and initialize it
        /// </summary>
        /// <param name="type">Plugin type</param>
        /// <param name="path">Plugin path</param>
        /// <param name="isUnloadable">Whether the plugin is unloadable</param>
        /// <returns>True whether everything ended successfully</returns>
        internal static bool CreatePlugin(Type type, string path, bool isUnloadable)
        {
            try
            {
                if (type == null || Interface.FindPlugin(type.Name) != null)
                    return false;

                if (Activator.CreateInstance(type) is Plugin plugin)
                {
                    plugin.Initialize(path, isUnloadable);
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

        /// <summary>
        /// Initialize a plugin
        /// </summary>
        /// <param name="path">Plugin path</param>
        /// <param name="isUnloadable">Whether the plugin is unloadable</param>
        internal void Initialize(string path, bool isUnloadable)
        {
            var type = GetType();

            Path = path;
            Filename = System.IO.Path.GetFileName(path);
            Name = type.Name;
            Title = type.Name;
            Author = "Unknown";
            Version = new Version(1, 0, 0);
            IsCorePlugin = string.IsNullOrEmpty(Path);
            IsUnloadable = isUnloadable;

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

                    if (field.GetCustomAttribute<VariableAttribute>(false) is VariableAttribute variableAttribute)
                    {
                        AddVariable(variableAttribute.Name, field);
                    }
                }

                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var propertiesCount = properties.Length;
                for (var j = 0; j < propertiesCount; j++)
                {
                    var property = properties[j];
                    if (property.GetCustomAttribute<VariableAttribute>(false) is VariableAttribute variableAttribute)
                    {
                        AddVariable(variableAttribute.Name, property);
                    }
                }
            }
            
            Interface.Plugins.Add(this);
            Interface.UpdateDependencies();
            Interface.Load(Name);
        }
        
        #region Dependencies

        /// <summary>
        /// Add a dependency
        /// </summary>
        /// <param name="name">Dependency (plugin) name</param>
        /// <param name="field">Field Info instance of the needed field</param>
        public void AddDependency(string name, FieldInfo field)
        {
            if (!Dependency.HasMatchingSignature(field))
            {
                Log.Warning($"Plugin {Title} tried to register a dependency for an incorrect field");
                return;
            }
            
            Dependencies.Add(new Dependency(this, name, field));
        }

        /// <summary>
        /// Update all dependencies
        /// </summary>
        public void UpdateDependencies()
        {
            if (!IsLoaded)
                return;
            
            Log.Debug($"Updating dependencies in plugin {Name}", 5);
            for (var i = 0; i < Dependencies.Count; i++)
            {
                Dependencies[i].Update();
            }
        }

        public List<Dependency> GetDependencies() => Dependencies;
        
        #endregion
        
        #region Hooks

        /// <summary>
        /// Call a specified hook
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args">Arguments</param>
        /// <returns>Result value</returns>
        private object CallHook(string name, params object[] args)
        {
            Log.Debug($"Calling a hook on {Name} ({name})", 8);
            
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
        /// Call a hook
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args">Arguments</param>
        /// <returns>Result value</returns>
        public object Call(string name, params object[] args) => CallHook(name, args);

        /// <summary>
        /// Call a hook with the specified return type
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args">Arguments</param>
        /// <typeparam name="T">Type of the return value</typeparam>
        /// <returns>Result value as T</returns>
        public T Call<T>(string name, params object[] args)
        {
            var result = Call(name, args);
            return result == null ? default : (T) Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Add a hook method to plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="method">Method Info instance of the needed method</param>
        public void AddHookMethod(string name, MethodInfo method)
        {
            if (Hooks.ContainsKey(name))
            {
                Log.Warning($"Plugin {Title} tried to register an existing hook");
                return;
            }
            
            Hooks[name] = new HookMethod(this, name, method);
        }

        /// <summary>
        /// Calls hooks if they exist
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args">Arguments</param>
        /// <returns>Result value</returns>
        private object OnCallHook(string name, object[] args)
        {
            var hook = FindHook(name, args);
            if (hook == null)
                return null;
            
            try
            {
                Log.Debug($"Calling an existing hook on {Name} ({name})", 6);
                return hook.Method?.Invoke(this, hook.FormatArguments(args));
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
        /// <param name="args">Arguments</param>
        /// <returns>Hook method instance</returns>
        public HookMethod FindHook(string name, object[] args) => !Hooks.TryGetValue(name, out var hook) || !hook.CanUseHook(this, args) ? null : hook;

        #endregion
        
        #region Commands

        /// <summary>
        /// Add a command to plugin
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="method">Method Info instance of the needed method</param>
        public void AddCommand(string name, MethodInfo method)
        {
            if (!Command.HasMatchingSignature(method))
            {
                Log.Warning($"Plugin {Title} tried to register a command with incorrect method arguments");
                return;
            }
            
            AddCommand(name, arg => method?.Invoke(this, new object[] {arg}));
        }
        
        /// <summary>
        /// Add a command to plugin
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="action">Action to be executed</param>
        public void AddCommand(string name, Action<CommandArgument> action)
        {
            name = name.ToLower();
            if (Commands.ContainsKey(name))
            {
                Log.Warning($"Plugin {Title} tried to register an existing command");
                return;
            }

            Commands[name] = new Command(this, name, action);
        }

        /// <summary>
        /// Add a command to plugin
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="memberInfo">Member Info instance of the needed field/property</param>
        public void AddVariable(string name, MemberInfo memberInfo)
        {
            name = name.ToLower();
            if (Commands.ContainsKey(name))
            {
                Log.Warning($"Plugin {Title} tried to register an existing command");
                return;
            }

            Commands[name] = new Command(this, name, memberInfo);
        }

        /// <summary>
        /// Find a matching command
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
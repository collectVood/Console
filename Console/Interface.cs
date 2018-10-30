using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Console.Plugins;

namespace Console
{
    public static class Interface
    {
        public static Controller Controller = Controller.Instance;
        
        public static List<Plugin> Plugins { get; } = PoolNew<List<Plugin>>.Get();
        
        /// <summary>
        /// Calls a specified hook on every plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object Call(string name, params object[] args)
        {
            try
            {
                var pluginsCount = Plugins.Count;
                var results = new object[pluginsCount];
                for (var i = 0; i < pluginsCount; i++)
                {
                    results[i] = Plugins[i]?.Call(name, args);
                }

                var conflicts = PoolNew<List<HookConflict>>.Get();
                object result = null;
                for (var i1 = 0; i1 < pluginsCount; i1++)
                {
                    for (var i2 = 0; i2 < pluginsCount; i2++)
                    {
                        if (i1 == i2)
                            continue;

                        var result1 = results[i1];
                        var result2 = results[i2];
                        if (result1 == null && result2 == null)
                            continue;

                        if (ReferenceEquals(result1, result2))
                            result = result1;
                        else
                            conflicts.Add(new HookConflict(Plugins[i1], Plugins[i2], result1, result2));
                    }
                }

                if (conflicts.Count > 0)
                    Log.Warning(
                        $"Calling hook '{name}' resulted in a conflict between the following plugins: {string.Join(", ", conflicts)}");

                return result;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return null;
        }

        /// <summary>
        /// Calls a specified hook on every plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Call<T>(string name, params object[] args)
        {
            var result = Call(name, args);
            return result == null ? default(T) : (T) Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Calls a specified hook on every plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        public static void CallHook(string name, params object[] args) => Call(name, args);

        internal static void LoadAssembly(string path)
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(path));
                var type = assembly.GetType("Console.Plugins." + Path.GetFileNameWithoutExtension(path));
                
                Plugin.CreatePlugin(type, path, false);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        internal static void UnloadAssembly(string path)
        {
            var pluginsCount = Plugins.Count;
            var plugin = (Plugin) null;
            for (var i = 0; i < pluginsCount; i++)
            {
                if (Plugins[i].Path == path)
                    plugin = Plugins[i];
            }

            if (plugin == null)
                return;

            Plugins.Remove(plugin);
            Log.Info($"Unloaded plugin {plugin.Title} from {plugin.Author} v{plugin.Version}");
        }
    }
}
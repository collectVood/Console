using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Console.Plugins;

namespace Console
{
    public static class Interface
    {
        public static Controller Controller = Controller.Instance;
        
        public static List<Plugin> Plugins { get; } = Pool<List<Plugin>>.Get();
        
        /// <summary>
        /// Calls a specified hook on every plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Call<T>(string name, params object[] args)
        {
            var pluginsCount = Plugins.Count;
            var results = Pool<T[]>.Get();
            for (var i = 0; i < pluginsCount; i++)
            {
                results[i] = Plugins[i].Call<T>(name, args);
            }

            var conflicts = Pool<List<HookConflict>>.Get();
            var result = Pool<T>.Get();
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
                Log.Warning($"Calling hook '{name}' resulted in a conflict between the following plugins: {string.Join(", ", conflicts)}");

            return result;
        }

        /// <summary>
        /// Calls a specified hook on every plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object Call(string name, params object[] args) => Call<object>(name, args);

        /// <summary>
        /// Calls a specified hook on every plugin
        /// </summary>
        /// <param name="name">Hook name</param>
        /// <param name="args"></param>
        public static void CallHook(string name, params object[] args) => Call(name, args);
    }
}
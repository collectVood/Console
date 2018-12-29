using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Console.Plugins;

namespace Console
{
    public class Controller
    {
        public static Controller Instance { get; private set; }
        public static ConsoleManager ConsoleManager { get; private set; }
        
        private FileSystemWatcher FSWatcherPlugins { get; set; }
        
        public Func<float> TimeSinceStartup { get; }
        private string RootDirectory { get; }
        private string PluginDirectory { get; }

        public float Now => TimeSinceStartup();

        private Stopwatch timer;

        public Controller()
        {
            Instance = this;
            
            RootDirectory = Environment.CurrentDirectory;
            PluginDirectory = Path.Combine(RootDirectory, "plugins");

            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);
            
            // File System Watchers
            FSWatcherPlugins = new FileSystemWatcher(PluginDirectory, "*.dll");
            FSWatcherPlugins.Created += (sender, args) =>
            {
                Interface.LoadAssembly(args.FullPath);
            };
            FSWatcherPlugins.Changed += (sender, args) =>
            {
                Interface.UnloadAssembly(args.FullPath);
                Interface.LoadAssembly(args.FullPath);
            };
            FSWatcherPlugins.Deleted += (sender, args) =>
            {
                Interface.UnloadAssembly(args.FullPath);
            };
            FSWatcherPlugins.Renamed += (sender, args) =>
            {
                Interface.UnloadAssembly(args.OldFullPath);
                Interface.LoadAssembly(args.FullPath);
            };
            FSWatcherPlugins.IncludeSubdirectories = false;
            FSWatcherPlugins.EnableRaisingEvents = true;
            GC.KeepAlive(FSWatcherPlugins);
            
            // Time since startup
            timer = new Stopwatch();
            timer.Start();
            TimeSinceStartup = () => (float) timer.Elapsed.TotalSeconds;
            
            // Initializing console
            ConsoleManager = new ConsoleManager();
            ConsoleManager.Initialize();
            
            // Loading core plugins
            Plugin.CreatePlugin(typeof(ConsoleBase), string.Empty, true);
        }
    }
}
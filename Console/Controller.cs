using System;
using System.Diagnostics;
using System.IO;
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
            FSWatcherPlugins.Created += OnFileChanged;
            FSWatcherPlugins.Changed += OnFileChanged;
            FSWatcherPlugins.Deleted += OnFileChanged;
            FSWatcherPlugins.IncludeSubdirectories = false;
            FSWatcherPlugins.EnableRaisingEvents = true;
            GC.KeepAlive(FSWatcherPlugins);
            
            // Time since startup
            timer = new Stopwatch();
            timer.Start();
            TimeSinceStartup = () => (float) timer.Elapsed.TotalSeconds;
            
            // Loading core plugins
            Plugin.CreatePlugin(typeof(ConsoleBase), string.Empty, true);
            
            ConsoleManager = new ConsoleManager();
            ConsoleManager.Initialize();
        }

        private void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            if (args == null)
                return;

            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Created:
                {
                    Interface.LoadAssembly(args.FullPath);
                    break;
                }

                case WatcherChangeTypes.Changed:
                case WatcherChangeTypes.Renamed:
                {
                    Interface.UnloadAssembly(args.FullPath);
                    Interface.LoadAssembly(args.FullPath);
                    break;
                }

                case WatcherChangeTypes.Deleted:
                {
                    Interface.UnloadAssembly(args.FullPath);
                    break;
                }
            }
        }
    }
}
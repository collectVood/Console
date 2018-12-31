using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Console.Plugins;
using Console.Plugins.Core;

namespace Console
{
    public class Controller
    {
        public static Controller Instance { get; private set; }
        public static ConsoleManager ConsoleManager { get; private set; }
        
        private FileSystemWatcher FSWatcherPlugins { get; }

        private List<Action> _nextTickQueue = new List<Action>();
        
        private Func<double> TimeSinceStartup { get; }
        private string RootDirectory { get; }
        private string PluginDirectory { get; }
        internal string LogDirectory { get; }

        public double Now => TimeSinceStartup();

        public Controller()
        {
            Instance = this;
            
            RootDirectory = Environment.CurrentDirectory;
            PluginDirectory = Path.Combine(RootDirectory, "plugins");
            LogDirectory = Path.Combine(RootDirectory, "logs");

            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            
            // Logging exceptions
//            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Log.Exception((Exception) eventArgs.ExceptionObject);
                Log.Warning("Fatal error! Exit in 5 sec.");
                
                System.Console.Beep(1000, 1000);
                Thread.Sleep(5000);
                Environment.Exit(0);
            };
            
            // File System Watchers
            FSWatcherPlugins = new FileSystemWatcher(PluginDirectory, "*.dll");
            FSWatcherPlugins.Created += OnFileCreated;
            FSWatcherPlugins.Changed += OnFileChanged;
            FSWatcherPlugins.Deleted += OnFileDeleted;
            FSWatcherPlugins.Renamed += OnFileRenamed;
            FSWatcherPlugins.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            FSWatcherPlugins.IncludeSubdirectories = false;
            FSWatcherPlugins.EnableRaisingEvents = true;
            GC.KeepAlive(FSWatcherPlugins);
            
            // Time since startup
            var timer = new Stopwatch();
            timer.Start();
            TimeSinceStartup = () => timer.Elapsed.TotalSeconds;
            
            // Initializing console
            ConsoleManager = new ConsoleManager();
            ConsoleManager.Initialize();

            // Loading core plugins
            Plugin.CreatePlugin(typeof(ConsoleBase), string.Empty);

            // Loading other available plugins
            var files = Directory.GetFiles(PluginDirectory);
            for (var i = 0; i < files.Length; i++)
            {
                var path = files[i];
                if (Path.GetExtension(path) == ".dll")
                {
                    Interface.LoadAssembly(path);
                }
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        internal void OnFrame()
        {
            for (var i = _nextTickQueue.Count - 1; i >= 0; i--)
            {
                try
                {
                    _nextTickQueue[i].Invoke();
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

            _nextTickQueue.Clear();
            
            Interface.PluginsQueue.Process();

            ConsoleManager.Update();
            Interface.CallHook("OnFrame");
        }
        
        #region File System Events

        private void OnFileCreated(object sender, FileSystemEventArgs args)
        {
            Interface.PluginsQueue.Enqueue(args.FullPath, PluginsQueueAction.Load);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            Interface.PluginsQueue.Enqueue(args.FullPath, PluginsQueueAction.Reload);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs args)
        {
            Interface.PluginsQueue.Enqueue(args.FullPath, PluginsQueueAction.Unload);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs args)
        {
            Interface.PluginsQueue.Enqueue(args.OldFullPath, PluginsQueueAction.Unload);
            Interface.PluginsQueue.Enqueue(args.FullPath, PluginsQueueAction.Load);
        }
        
        #endregion
        
        #region Methods

        public void NextFrame(Action action)
        {
            _nextTickQueue.Add(action);
        } 
        
        #endregion
    }
}
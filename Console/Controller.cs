using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Console.Files;
using Console.Plugins;
using Console.Plugins.Core;
using Timer = Console.Plugins.Timers.Timer;
using Version = Console.Plugins.Version;

namespace Console
{
    public class Controller
    {
        public static Controller Instance { get; private set; }
        public static ConsoleManager ConsoleManager { get; private set; }
        
        public bool IsRunning { get; private set; }

        private FileSystemWatcher FSWatcherPlugins { get; }
        public DataFileSystem DataFileSystem { get; }

        private List<Action> _nextTickQueue = new List<Action>();
        
        private Func<double> TimeSinceStartup { get; }
        public string RootDirectory { get; }
        public string PluginDirectory { get; }
        public string LogDirectory { get; }
        public string LanguageDirectory { get; }
        public string DataDirectory { get; }
        public string DataTemporaryDirectory { get; }

        public int DebugLevel = 0;

        public double Now => TimeSinceStartup();
        
        public Version Version { get; }

        public Controller()
        {
            Instance = this;
            IsRunning = true;
            
            // Time since startup
            var timer = new Stopwatch();
            timer.Start();
            TimeSinceStartup = () => timer.Elapsed.TotalSeconds;
            
            RootDirectory = Environment.CurrentDirectory;
            PluginDirectory = Path.Combine(RootDirectory, "plugins");
            LogDirectory = Path.Combine(RootDirectory, "logs");
            LanguageDirectory = Path.Combine(RootDirectory, "lang");
            DataDirectory = Path.Combine(RootDirectory, "data");
            DataTemporaryDirectory = Path.Combine(DataDirectory, "Temporary");

            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            if (!Directory.Exists(LanguageDirectory))
                Directory.CreateDirectory(LanguageDirectory);
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
            if (!Directory.Exists(DataTemporaryDirectory))
                Directory.CreateDirectory(DataTemporaryDirectory);
            
            DataFileSystem = new DataFileSystem(DataDirectory);
            
            // Logging exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Log.Exception((Exception) eventArgs.ExceptionObject);
                Log.Error("Exit in 5 seconds");
                
                System.Console.Beep(1000, 1000);
                Thread.Sleep(5000);
                IsRunning = false;
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
            
            // Version setup
            Version = new Version(Assembly.GetExecutingAssembly());
            
            // Initializing console
            ConsoleManager = new ConsoleManager();
            
            // Loading core plugins
            Plugin.CreatePlugin(typeof(Core), string.Empty, false);
            
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
            
            Interface.CallHook("OnInitialized");
        }

        /// <summary>
        /// On every frame
        /// </summary>
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
            Timer.Process();

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

        /// <summary>
        /// Execute action in the next frame
        /// </summary>
        /// <param name="action">Action to execute</param>
        public void NextFrame(Action action)
        {
            _nextTickQueue.Add(action);
        } 
        
        #endregion
    }
}
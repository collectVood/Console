using System;
using System.Diagnostics;

namespace Console
{
    public class Controller
    {
        public static Controller Instance { get; private set; }
        public static ConsoleManager ConsoleManager { get; private set; }
        
        public Func<float> TimeSinceStartup { get; }
        private string RootDirectory { get; }

        public float Now => TimeSinceStartup();

        private Stopwatch timer;

        public Controller()
        {
            Instance = this;
            
            RootDirectory = Environment.CurrentDirectory;
            
            // Time since startup
            timer = new Stopwatch();
            timer.Start();
            TimeSinceStartup = () => (float) timer.Elapsed.TotalSeconds;
            
            ConsoleManager = new ConsoleManager();
            ConsoleManager.Initialize();
        }
    }
}
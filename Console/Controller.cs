using System;
using System.Diagnostics;
using System.Threading;

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

            StartFrames();
        }

        private void StartFrames()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    ConsoleManager.Update();
                    Interface.CallHook("OnFrame");
                }
            });
        }
    }
}
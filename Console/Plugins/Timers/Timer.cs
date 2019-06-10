using System;
using System.Collections.Generic;

namespace Console.Plugins.Timers
{
    public class Timer
    {
        #region Timer

        public Plugin Owner { get; }

        public Action Callback { get; }

        public double Delay { get; }

        public double LastRun { get; private set; } = Interface.Controller.Now;

        public bool Repeat { get; }
        
        public bool IsDisabled { get; private set; }

        private Timer(Plugin owner, Action callback, double delay, bool repeat)
        {
            Owner = owner;
            Callback = callback;
            Delay = delay;
            Repeat = repeat;
            IsDisabled = false;
            
            _timers.Add(this);
        }

        /// <summary>
        /// Returns true if the timer has expired
        /// </summary>
        /// <returns>True if the timer has expired</returns>
        private bool Expired() => Interface.Controller.Now - LastRun >= Delay;

        /// <summary>
        /// Runs the timer action
        /// </summary>
        private void Run()
        {
            Callback.Invoke();
            LastRun = Interface.Controller.Now;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            IsDisabled = true;
        }
        
        #endregion

        #region Static
        
        private static List<Timer> _timers = PoolNew<List<Timer>>.Get();
        
        /// <summary>
        /// Process all the timers
        /// </summary>
        internal static void Process()
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                if (timer.IsDisabled || timer.Owner != null && !timer.Owner.IsLoaded)
                {
                    _timers.RemoveAt(i);
                    continue;
                }

                if (!timer.Expired()) continue;
                
                timer.Run();
                if (timer.Repeat)
                    continue;
                
                _timers.RemoveAt(i);
            }
        }

        /// <summary>
        /// Create a new timer
        /// </summary>
        /// <param name="owner">Timer's plugin-owner</param>
        /// <param name="delay">Delay between timer runs</param>
        /// <param name="repeat">Repeat the timer</param>
        /// <param name="callback">Callback</param>
        /// <returns>Created timer instance</returns>
        public static Timer Create(Plugin owner, double delay, bool repeat, Action callback)
        {
            if (callback == null || delay <= 0)
                return null;
            
            return new Timer(owner, callback, delay, repeat);
        }
        
        #endregion
    }
}
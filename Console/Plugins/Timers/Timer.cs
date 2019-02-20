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

        private bool Expired() => Interface.Controller.Now - LastRun >= Delay;

        private void Run()
        {
            if (Repeat)
                LastRun = Interface.Controller.Now;
            
            Callback.Invoke();
        }

        public void Stop()
        {
            IsDisabled = true;
        }
        
        #endregion

        #region Static
        
        private static List<Timer> _timers = PoolNew<List<Timer>>.Get();
        
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

        public static Timer Create(Plugin owner, double delay, bool repeat, Action callback)
        {
            if (callback == null || delay <= 0)
                return null;
            
            return new Timer(owner, callback, delay, repeat);
        }
        
        #endregion
    }
}
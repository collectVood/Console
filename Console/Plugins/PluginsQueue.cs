using System.Collections.Generic;

namespace Console.Plugins
{
    internal class PluginsQueue
    {
        internal List<PluginsQueueEntry> Entries = new List<PluginsQueueEntry>();
        internal double Interval = 1.0d;

        /// <summary>
        /// Add a new plugin to the queue
        /// </summary>
        /// <param name="path">Plugin file path</param>
        /// <param name="action">Queued action</param>
        internal void Enqueue(string path, PluginsQueueAction action)
        {
            Log.Debug($"Adding plugin to queue ({action}, {path})", 6);
            
            var entry = PluginsQueueEntry.Find(path);
            if (entry != null && entry.Action == action)
            {
                entry.TimeAdded = Interface.Controller.Now;
            }
            else
            {
                entry = new PluginsQueueEntry(path, action);
                Entries.Add(entry);
            }
        }
        
        /// <summary>
        /// Process queued actions
        /// </summary>
        internal void Process()
        {
            for (var i = Entries.Count - 1; i >= 0; i--)
            {
                var entry = Entries[i];
                if (Interface.Controller.Now - entry.TimeAdded < Interval)
                    continue;

                entry.Process();
                Entries.RemoveAt(i);
            }
        }

        internal int Count() => Entries.Count;
    }

    internal class PluginsQueueEntry
    {
        internal string Path { get; }
        internal PluginsQueueAction Action { get; set; }
        internal double TimeAdded { get; set; } = Interface.Controller.Now;

        internal PluginsQueueEntry(string path, PluginsQueueAction action)
        {
            Path = path;
            Action = action;
        }

        /// <summary>
        /// Process current entry
        /// </summary>
        internal void Process()
        {
            Log.Debug($"Processing plugins queue entry ({Action}, {Path})", 6);
            
            switch (Action)
            {
                case PluginsQueueAction.Load:
                {
                    Interface.LoadAssembly(Path);
                    break;
                }

                case PluginsQueueAction.Unload:
                {
                    Interface.UnloadAssembly(Path);
                    break;
                }

                case PluginsQueueAction.Reload:
                {
                    Interface.UnloadAssembly(Path);
                    Interface.LoadAssembly(Path);
                    break;
                }
            }
        }

        /// <summary>
        /// Find a queue entry by plugin file path
        /// </summary>
        /// <param name="path">Plugin file path</param>
        /// <returns>Plugin queue entry instance</returns>
        internal static PluginsQueueEntry Find(string path)
        {
            for (var i = 0; i < Interface.PluginsQueue.Entries.Count; i++)
            {
                var entry = Interface.PluginsQueue.Entries[i];
                if (entry.Path == path)
                    return entry;
            }

            return null;
        }
    }

    internal enum PluginsQueueAction
    {
        Load,
        Unload,
        Reload
    }
}
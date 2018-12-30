using System.Collections.Generic;

namespace Console.Plugins
{
    internal class PluginsQueue
    {
        internal List<PluginsQueueEntry> Entries = new List<PluginsQueueEntry>();
        internal double Interval = 1.0d;

        internal void Enqueue(string path, PluginsQueueAction action)
        {
            var entry = PluginsQueueEntry.Find(path);
            if (entry != null)
                Entries.Remove(entry);
            
            Entries.Add(new PluginsQueueEntry(path, action));
        }
        
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
        internal PluginsQueueAction Action { get; }
        internal double TimeAdded { get; } = Interface.Controller.Now;

        internal PluginsQueueEntry(string path, PluginsQueueAction action)
        {
            Path = path;
            Action = action;
        }

        internal void Process()
        {
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
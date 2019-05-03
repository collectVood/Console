using System;
using System.Collections.Generic;

namespace Console.Files
{
    public static class FileCache
    {
        private static List<File> _cache;

        public static File Get(string path)
        {
            var file = Find(path);
            if (file != null)
                return file;

            file = new File(path);
            _cache.Add(file);
            return file;
        }

        public static void Add(File file)
        {
            if (Find(file.FilePath) != null)
                return;

            _cache.Add(file);
        }

        public static void Remove(int index)
        {
            if (_cache.Count <= index || index < 0)
                return;

            _cache.RemoveAt(index);
        }

        public static File Find(string path)
        {
            for (var i = 0; i < _cache.Count; i++)
            {
                var file = _cache[i];
                if (file.FilePath == path)
                    return file;
            }

            return null;
        }

        public static void RemoveDuplicates()
        {
            var paths = new HashSet<string>();
            
            for (var i = _cache.Count - 1; i >= 0; i--)
            {
                var path = _cache[i].FilePath;
                if (paths.Contains(path))
                {
                    Remove(i);
                }
                else
                {
                    paths.Add(path);
                }
            }
            
            GC.Collect();
        }
    }
}
using System;
using System.Collections.Generic;

namespace Console.Files
{
    public static class FileCache
    {
        private static List<File> _cache = new List<File>();

        /// <summary>
        /// Get file instance by path
        /// </summary>
        /// <param name="path">File full path</param>
        /// <returns>File instance</returns>
        public static File Get(string path)
        {
            var file = Find(path);
            if (file != null)
                return file;

            file = new File(path);
            _cache.Add(file);
            return file;
        }

        /// <summary>
        /// Add new file to cache
        /// </summary>
        /// <param name="file">File instance</param>
        public static void Add(File file)
        {
            if (Find(file.FilePath) != null)
                return;

            _cache.Add(file);
        }

        /// <summary>
        /// Remove file by index from cache
        /// </summary>
        /// <param name="index">File index in cache</param>
        public static void Remove(int index)
        {
            if (_cache.Count <= index || index < 0)
                return;

            _cache.RemoveAt(index);
        }

        /// <summary>
        /// Find file instance in cache by it's path
        /// </summary>
        /// <param name="path">File full path</param>
        /// <returns>File instance or null if not found</returns>
        public static File Find(string path)
        {
            var index = FindIndex(path);
            if (index.HasValue)
                return _cache[index.Value];

            return null;
        }

        /// <summary>
        /// Find file index in cache by it's path
        /// </summary>
        /// <param name="path">File full path</param>
        /// <returns>Index or null if not found</returns>
        public static int? FindIndex(string path)
        {
            for (var i = 0; i < _cache.Count; i++)
            {
                if (_cache[i].FilePath == path)
                    return i;
            }

            return null;
        }

        /// <summary>
        /// Remove files with duplicate paths
        /// </summary>
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
using System;
using System.IO;
using Newtonsoft.Json;

namespace Console
{
    public class DataFileSystem
    {
        private static object _lock = new object();
        
        public string Directory { get; }

        public DataFileSystem(string directory)
        {
            Directory = directory;
        }

        public T ReadObject<T>(string filename)
        {
            lock (_lock)
            {
                var path = Path.Combine(Directory, filename);
                return Exists(path)
                    ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path))
                    : Activator.CreateInstance<T>();
            }
        }

        public void WriteObject<T>(T obj, string filename)
        {
            lock (_lock)
            {
                var path = Path.Combine(Directory, filename);
                Exists(path, true);

                var text = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(path, text);
            }
        }

        public static bool Exists(string path, bool create = false)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
                {
                    if (create)
                        System.IO.Directory.CreateDirectory(directory);
                    else
                        return false;
                }

                // ReSharper disable once InvertIf
                if (!File.Exists(path))
                {
                    if (create)
                        File.Create(path).Close();
                    else
                        return false;
                }

                return true;
            }
        }
    }
}
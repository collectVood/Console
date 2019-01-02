using System;
using System.IO;
using Newtonsoft.Json;

namespace Console
{
    public class DataFileSystem
    {
        public string Directory { get; }

        public DataFileSystem(string directory)
        {
            Directory = directory;
        }

        public T ReadObject<T>(string filename)
        {
            var path = Path.Combine(Directory, filename);
            return Exists(path)
                ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path))
                : Activator.CreateInstance<T>();
        }

        public void WriteObject<T>(T obj, string filename)
        {
            var path = Path.Combine(Directory, filename);
            Exists(path, true);

            var text = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(path, text);
        }

        public static bool Exists(string path, bool create = false)
        {
            var file = new FileInfo(path);
            var dir = file.Directory;

            if (!create) return dir != null && dir.Exists && file.Exists;
            
            if (dir != null && !dir.Exists)
                System.IO.Directory.CreateDirectory(dir.FullName);
            
            if (!file.Exists)
                File.Create(path);

            return true;

        }
    }
}
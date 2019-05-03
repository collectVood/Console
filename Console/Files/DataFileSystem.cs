using System.IO;

namespace Console.Files
{
    public class DataFileSystem
    {
        public string Directory { get; }

        public DataFileSystem(string directory)
        {
            Directory = directory;
        }

        public T Read<T>(string file)
        {
            return new File(Path.Combine(Directory, file)).Read<T>();
        }

        public void Write<T>(T obj, string file)
        {
            new File(Path.Combine(Directory, file)).Write(obj);
        }

        public static bool Exists(string path, bool create = false) => new File(path).Exists(create);
    }
}
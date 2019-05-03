using System.IO;

namespace Console.Files
{
    public class DataFileSystem
    {
        #region Variables
        
        public string Directory { get; }
        
        #endregion

        public DataFileSystem(string directory)
        {
            Directory = directory;
        }
        
        #region Methods

        public T Read<T>(string file)
        {
            return new File(Path.Combine(Directory, file)).Read<T>();
        }

        public void Write<T>(T obj, string file)
        {
            new File(Path.Combine(Directory, file)).Write(obj);
        }

        public static bool Exists(string path, bool create = false) => new File(path).Exists(create);
        
        #endregion
        
        #region Helpers

        private string GetPath(string file)
        {
            var isRooted = Path.IsPathRooted(file);
            return isRooted ? Path.Combine(Directory, file) : file;
        }
        
        #endregion
    }
}
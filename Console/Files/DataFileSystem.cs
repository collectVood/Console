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
            return FileCache.Get(GetPath(file)).Read<T>();
        }

        public void Write(object obj, string file)
        {
            FileCache.Get(GetPath(file)).Write(obj);
        }

        public bool Exists(string file, bool create = false) => FileCache.Get(GetPath(file)).Exists(create);
        
        #endregion
        
        #region Helpers

        public string GetPath(string file)
        {
            var isRooted = Path.IsPathRooted(file);
            return isRooted ? file : Path.Combine(Directory, file);
        }
        
        #endregion
    }
}
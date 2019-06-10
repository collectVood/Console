using System.IO;

namespace Console.Files
{
    public class DataFileSystem
    {
        #region Variables
        
        public string Directory { get; }
        
        #endregion

        /// <summary>
        /// Create a new DataFileSystem
        /// </summary>
        /// <param name="directory">Directory of DFS</param>
        public DataFileSystem(string directory)
        {
            Directory = directory;
        }
        
        #region Methods

        /// <summary>
        /// Read T object from the specified data file
        /// </summary>
        /// <param name="file">File name</param>
        /// <typeparam name="T">Type of the return value</typeparam>
        /// <returns></returns>
        public T Read<T>(string file)
        {
            return FileCache.Get(GetPath(file)).Read<T>();
        }

        /// <summary>
        /// Write object to the specified file
        /// </summary>
        /// <param name="obj">Object to write</param>
        /// <param name="file">File name</param>
        public void Write(object obj, string file)
        {
            FileCache.Get(GetPath(file)).Write(obj);
        }

        public bool Exists(string file, bool create = false) => FileCache.Get(GetPath(file)).Exists(create);
        
        #endregion
        
        #region Helpers

        /// <summary>
        /// Get full path by file name or an another one full path
        /// </summary>
        /// <param name="file">File path</param>
        /// <returns>Full path to the file</returns>
        public string GetPath(string file)
        {
            var isRooted = Path.IsPathRooted(file);
            return isRooted ? file : Path.Combine(Directory, file);
        }
        
        #endregion
    }
}
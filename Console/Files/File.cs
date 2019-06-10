using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Console.Files
{
    public class File
    {
        #region Variables
        
        private static object _lock = new object();

        private static BinaryFormatter _binaryFormatter = new BinaryFormatter();
        
        /// <summary>
        /// Path to the file
        /// </summary>
        public string FilePath { get; }
        
        /// <summary>
        /// Returns true if the file is in JSON (.json) format
        /// </summary>
        public bool IsJson { get; }
        
        /// <summary>
        /// Returns true if the file is in BINARY (.data) format
        /// </summary>
        public bool IsBinary { get; }
        
        #endregion

        /// <summary>
        /// Creates a new File instance
        /// </summary>
        /// <param name="path">Path to the file</param>
        public File(string path)
        {
            var extension = Path.GetExtension(path);
            
            IsJson = extension == ".json";
            IsBinary = extension == ".data";
            
            FilePath = path;
        }
        
        #region Methods

        /// <summary>
        /// Does the file exist
        /// </summary>
        /// <param name="create">Create file if it doesn't exist if true</param>
        /// <returns>Returns whether the file exists</returns>
        public bool Exists(bool create)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(FilePath))
                    return false;

                var directory = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    if (create)
                        Directory.CreateDirectory(directory);
                    else
                        return false;
                }

                if (System.IO.File.Exists(FilePath))
                    return true;

                if (create)
                    System.IO.File.Create(FilePath).Close();
                else
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Write object to the file
        /// </summary>
        /// <param name="obj">Object to write</param>
        public void Write(object obj)
        {
            Exists(true);

            lock (_lock)
            {
                if (IsJson)
                    WriteJson(obj);

                if (IsBinary)
                    WriteBinary(obj);
            }
        }

        /// <summary>
        /// Read T object from the file
        /// </summary>
        /// <typeparam name="T">Type of the return value</typeparam>
        /// <returns>T instance from file</returns>
        public T Read<T>()
        {
            if (!Exists(false))
                return Converter.GetInstanceOf<T>();
            
            lock (_lock)
            {
                if (IsJson)
                    return ReadJson<T>();
                
                if (IsBinary)
                    return ReadBinary<T>();
            }

            return Converter.GetInstanceOf<T>(); // :P
        }

        private void WriteJson(object obj)
        {
            var text = JsonConvert.SerializeObject(obj, Formatting.Indented);
            System.IO.File.WriteAllText(FilePath, text);
        }

        private void WriteBinary(object obj)
        {
            using (var fs = new FileStream(FilePath, FileMode.Truncate))
            {
                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write(Converter.GetBytesFromObject(obj));
                }
            }
        }

        private T ReadJson<T>()
        {
            var data = System.IO.File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<T>(data);
        }

        private T ReadBinary<T>()
        {
            var bytes = System.IO.File.ReadAllBytes(FilePath);
            return Converter.GetObjectFromBytes<T>(bytes);
        }
        
        #endregion
    }
}
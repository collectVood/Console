using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Console
{
    public static class Converter
    {
        private static BinaryFormatter _binaryFormatter = new BinaryFormatter();
        
        /// <summary>
        /// Get bytes from object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Bytes</returns>
        public static byte[] GetBytesFromObject(object obj)
        {
            using (var ms = new MemoryStream())
            {
                _binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Get object with the specified type from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <typeparam name="T">Type to return</typeparam>
        /// <returns>Object of T type</returns>
        public static T GetObjectFromBytes<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return (T) _binaryFormatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Gets an instance of the specified type
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <returns>Instance of the specified type</returns>
        public static T GetInstanceOf<T>() => Activator.CreateInstance<T>();
    }
}
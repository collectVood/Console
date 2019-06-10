using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Console
{
    public static class Converter
    {
        private static BinaryFormatter _binaryFormatter = new BinaryFormatter();
        
        public static byte[] GetBytesFromObject(object obj)
        {
            using (var ms = new MemoryStream())
            {
                _binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T GetObjectFromBytes<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return (T) _binaryFormatter.Deserialize(ms);
            }
        }

        public static T GetInstanceOf<T>() => Activator.CreateInstance<T>();
    }
}
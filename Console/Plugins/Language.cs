using System.Collections.Generic;

namespace Console.Plugins
{
    public static class Language
    {
        #region Variables
        
        internal static DataFileSystem DataFileSystem = new DataFileSystem(Interface.Controller.LanguageDirectory);
        
        #endregion
        
        #region Messages

        public static void AddMessage(string file, string key, string message)
        {
            Log.Debug($"Adding message ({message}) to {file} with key {key}", 4);
            var messages = GetMessages(file);
            if (messages == null)
                messages = new Dictionary<string, string>();

            messages[key] = message;
            DataFileSystem.WriteObject(messages, file);
        }

        public static Dictionary<string, string> GetMessages(string file)
        {
            return DataFileSystem.ReadObject<Dictionary<string, string>>(file);
        }

        public static string GetMessage(string file, string key)
        {
            var messages = GetMessages(file);
            if (messages == null)
                return null;

            return messages.TryGetValue(key, out var message) ? message : null;
        }

        #endregion
    }
}
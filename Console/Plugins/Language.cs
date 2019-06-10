using System.Collections.Generic;
using System.IO;
using Console.Files;

namespace Console.Plugins
{
    public static class Language
    {
        #region Variables

        private static DataFileSystem _dataFileSystem = new DataFileSystem(Interface.Controller.LanguageDirectory);
        
        #endregion
        
        #region Messages

        /// <summary>
        /// Add a new lang message
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="file">File name</param>
        /// <param name="key">Message key</param>
        /// <param name="message">Message</param>
        public static void AddMessage(string lang, string file, string key, string message)
        {
            var messages = GetMessages(lang, file) ?? new Dictionary<string, string>();
            messages[key] = message;
            _dataFileSystem.Write(messages, file);
        }

        /// <summary>
        /// Get all messages from the specified language and file
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="file">File name</param>
        /// <returns>Message keys and messages</returns>
        public static Dictionary<string, string> GetMessages(string lang, string file)
        {
            return _dataFileSystem.Read<Dictionary<string, string>>(GetLanguageFile(lang, file));
        }

        /// <summary>
        /// Get a message from the specified language and file with the specified key
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="file">File name</param>
        /// <param name="key">Message key</param>
        /// <returns>Message</returns>
        public static string GetMessage(string lang, string file, string key)
        {
            var messages = GetMessages(lang, file);
            if (messages == null)
                return null;

            return messages.TryGetValue(key, out var message) ? message : null;
        }

        /// <summary>
        /// Get a language file name with the language directory
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="file">File name</param>
        /// <returns>Languaged file name</returns>
        private static string GetLanguageFile(string lang, string file)
        {
            return Path.Combine(lang, file);
        }

        #endregion
    }
}
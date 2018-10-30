using System;

namespace Console
{
    public static class Log
    {
        internal const ConsoleColor ColorDefault = ConsoleColor.Gray;
        internal const ConsoleColor ColorWarning = ConsoleColor.Yellow;
        internal const ConsoleColor ColorError = ConsoleColor.Red;
        internal const ConsoleColor ColorException = ConsoleColor.Red;
        internal const ConsoleColor ColorDebug = ConsoleColor.Gray;
        internal const ConsoleColor ColorInfo = ConsoleColor.Gray;
        internal const ConsoleColor ColorInput = ConsoleColor.Green;
        internal const ConsoleColor ColorBackground = ConsoleColor.Black;
        
        /// <summary>
        /// Prints warning to console
        /// </summary>
        /// <param name="input"></param>
        public static void Warning(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            Controller.ConsoleManager.AddMessage(input, ColorWarning);
        }
        
        /// <summary>
        /// Prints error to console
        /// </summary>
        /// <param name="input"></param>
        public static void Error(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            Controller.ConsoleManager.AddMessage(input, ColorError);
        }

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="e"></param>
        public static void Exception(Exception e)
        {
            Controller.ConsoleManager.AddMessage(e.ToString(), ColorException);
        }
        
        /// <summary>
        /// Prints debug text to console
        /// </summary>
        /// <param name="input"></param>
        public static void Debug(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            Controller.ConsoleManager.AddMessage("[DEBUG] " + input, ColorDebug);
        }
        
        /// <summary>
        /// Prints informational text to console
        /// </summary>
        /// <param name="input"></param>
        public static void Info(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            Controller.ConsoleManager.AddMessage(input, ColorInfo);
        }

        /// <summary>
        /// Prints text to console
        /// </summary>
        /// <param name="input"></param>
        internal static void Write(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            Controller.ConsoleManager.AddMessage(input, ColorDefault);
        }

        public static string Read() => System.Console.ReadLine();
        public static char ReadKey() => System.Console.ReadKey().KeyChar;
    }
}
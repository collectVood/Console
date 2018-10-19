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
            System.Console.ForegroundColor = ColorWarning;
            Write(input);
            System.Console.ResetColor();
        }
        
        /// <summary>
        /// Prints error to console
        /// </summary>
        /// <param name="input"></param>
        public static void Error(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.ForegroundColor = ColorError;
            Write(input);
        }
        
        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="input"></param>
        public static void Exception(Exception e)
        {
            if (e?.ToString() == null) return;
            System.Console.ForegroundColor = ColorException;
            Write(e.ToString());
        }
        
        /// <summary>
        /// Prints debug text to console
        /// </summary>
        /// <param name="input"></param>
        public static void Debug(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.ForegroundColor = ColorDebug;
            Write("[DEBUG] " + input);
        }
        
        /// <summary>
        /// Prints informational text to console
        /// </summary>
        /// <param name="input"></param>
        public static void Info(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.ForegroundColor = ColorInfo;
            Write(input);
        }

        /// <summary>
        /// Prints text to console
        /// </summary>
        /// <param name="input"></param>
        internal static void Write(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.ForegroundColor = ColorDefault;
            System.Console.WriteLine(input);
        }

        public static string Read() => System.Console.ReadLine();
        public static char ReadKey() => System.Console.ReadKey().KeyChar;
    }
}
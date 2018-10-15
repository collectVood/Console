using System;

namespace Console
{
    public class Log
    {
        /// <summary>
        /// Prints warning to console
        /// </summary>
        /// <param name="input"></param>
        public static void Warning(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
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
            System.Console.ForegroundColor = ConsoleColor.Red;
            Write(input);
            System.Console.ResetColor();
        }
        
        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="input"></param>
        public static void Exception(Exception e)
        {
            // TODO
        }
        
        /// <summary>
        /// Prints informational text to console
        /// </summary>
        /// <param name="input"></param>
        public static void Debug(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            Write(input);
            System.Console.ResetColor();
        }

        /// <summary>
        /// Prints text to console
        /// </summary>
        /// <param name="input"></param>
        internal static void Write(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            System.Console.WriteLine(input);
        }

        public static string Read() => System.Console.ReadLine();
        public static char ReadKey() => System.Console.ReadKey().KeyChar;
    }
}
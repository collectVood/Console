using System;

namespace Console
{
    public static class Log
    {
        /// <summary>
        /// output a warning in the console
        /// </summary>
        /// <param name="warning"></param>
        public static void Warning(string warning) 
        {
            if (string.IsNullOrEmpty(warning)) return;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("WARNING: " + warning);
            System.Console.ResetColor();
        }
        
        /// <summary>
        /// output an error in the console
        /// </summary>
        /// <param name="error"></param>
        public static void Error(string error)
        {
            if (string.IsNullOrEmpty(error)) return;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("ERROR: " + error);
            System.Console.ResetColor();
        }
        
        /// <summary>
        /// output a debug in the console
        /// </summary>
        /// <param name="debug"></param>
        public static void Debug(string debug)
        {
            if (string.IsNullOrEmpty(debug)) return;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine("DEBUG: " + debug);
            System.Console.ResetColor();
        }
        
        /// <summary>
        /// reads input from the console and returns a string
        /// </summary>
        /// <returns></returns>
        public static string Read()
        {
            var read = System.Console.ReadLine();
            return string.IsNullOrEmpty(read) ? "Input error!" : read;
        }
    }
}
using System;

namespace Console
{
    public static class Log
    {
        public static void Warning(string warning) 
        {
            if (string.IsNullOrEmpty(warning)) return;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("WARNING: " + warning);
            System.Console.ResetColor();
        }
        
        public static void Error(string error)
        {
            if (string.IsNullOrEmpty(error)) return;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("ERROR: " + error);
            System.Console.ResetColor();
        }
        
        public static void Debug(string debug)
        {
            if (string.IsNullOrEmpty(debug)) return;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine("DEBUG: " + debug);
            System.Console.ResetColor();
        }
        
        public static string Read()
        {
            var read = System.Console.ReadLine();
            return string.IsNullOrEmpty(read) ? "Input error!" : read;
        }
    }
}
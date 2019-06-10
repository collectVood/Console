using System;
using System.IO;

namespace Console
{
    public static class Log
    {
        private static object _lock = new object();
        
        internal const ConsoleColor ColorDefault = ConsoleColor.Gray;
        internal const ConsoleColor ColorWarning = ConsoleColor.Yellow;
        internal const ConsoleColor ColorError = ConsoleColor.Red;
        internal const ConsoleColor ColorException = ConsoleColor.Red;
        internal const ConsoleColor ColorDebug = ConsoleColor.Gray;
        internal const ConsoleColor ColorInfo = ConsoleColor.Gray;
        internal const ConsoleColor ColorInput = ConsoleColor.Green;
        internal const ConsoleColor ColorBackground = ConsoleColor.Black;

        internal static string LogPathAll => Path.Combine(Interface.Controller.LogDirectory, "All");
        internal static string LogPathWarning => Path.Combine(Interface.Controller.LogDirectory, "Warning");
        internal static string LogPathError => Path.Combine(Interface.Controller.LogDirectory, "Error");
        internal static string LogPathException => Path.Combine(Interface.Controller.LogDirectory, "Exception");
        internal static string LogPathDebug => Path.Combine(Interface.Controller.LogDirectory, "Debug");
        internal static string LogPathInfo => Path.Combine(Interface.Controller.LogDirectory, "Info");
        internal static string LogPathInput => Path.Combine(Interface.Controller.LogDirectory, "Input");

        internal const string LogFileName = "Log";
        
        /// <summary>
        /// Print warning to console
        /// </summary>
        /// <param name="input">Input</param>
        public static void Warning(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            LogFile($"{LogPathWarning}/{LogFileName}", input);
            Controller.ConsoleManager.AddMessage(input, ColorWarning);
        }
        
        /// <summary>
        /// Print error to console
        /// </summary>
        /// <param name="input">Input</param>
        public static void Error(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            LogFile($"{LogPathError}/{LogFileName}", input);
            Controller.ConsoleManager.AddMessage(input, ColorError);
        }

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="e">Exception</param>
        public static void Exception(Exception e)
        {
            var input = e?.ToString();
            if (string.IsNullOrEmpty(input)) return;
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            LogFile($"{LogPathException}/{LogFileName}", input);
            Controller.ConsoleManager.AddMessage(input, ColorException);
        }

        /// <summary>
        /// Print debug text to console
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="level">Debug level</param>
        public static void Debug(string input, int level = 1)
        {
            if (string.IsNullOrEmpty(input) || Interface.Controller.DebugLevel < level) return;
            input = $"[DEBUG] {input}";
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            LogFile($"{LogPathDebug}/{LogFileName}", input);
            Controller.ConsoleManager.AddMessage(input, ColorDebug);
        }
        
        /// <summary>
        /// Print informational text to console
        /// </summary>
        /// <param name="input">Input</param>
        public static void Info(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            LogFile($"{LogPathInfo}/{LogFileName}", input);
            Controller.ConsoleManager.AddMessage(input, ColorInfo);
        }

        /// <summary>
        /// Print text to console
        /// </summary>
        /// <param name="input">Input</param>
        internal static void Write(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            Controller.ConsoleManager.AddMessage(input, ColorDefault);
        }

        /// <summary>
        /// Log input
        /// </summary>
        /// <param name="input"></param>
        internal static void Input(string input)
        {
            if (string.IsNullOrEmpty(input)) return;
            
            LogFile($"{LogPathAll}/{LogFileName}", input);
            LogFile($"{LogPathInput}/{LogFileName}", input);
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <returns>String</returns>
        public static string Read() => System.Console.ReadLine();
        
        /// <summary>
        /// Reads a key (char)
        /// </summary>
        /// <returns>Char</returns>
        public static char ReadKey() => System.Console.ReadKey().KeyChar;

        /// <summary>
        /// Log text to a file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="text">Text</param>
        /// <param name="timeStamp">Add time stamp to the file name</param>
        public static void LogFile(string filename, string text, bool timeStamp = true)
        {
            filename = string.Concat(new[]
            {
                filename,
                timeStamp ? " " + DateTime.Now.ToString("yyyy-MM-dd") : string.Empty,
                ".txt"
            });

            filename = Path.Combine(Interface.Controller.LogDirectory, filename);

            TryLogFile(filename, text);
        }

        /// <summary>
        /// Try to log text to a file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="text">Text</param>
        private static void TryLogFile(string path, string text)
        {
            try
            {
                lock (_lock)
                {
                    var file = new FileInfo(path);
                    var directory = file.Directory;
                    if (directory == null)
                        return;

                    if (!directory.Exists)
                        Directory.CreateDirectory(directory.FullName);

                    using (var writer = new StreamWriter(path, true))
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            catch (Exception)
            {
                Interface.Controller.NextFrame(() => TryLogFile(path, text));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    public class ConsoleManager
    {
        #region Variables

//        public string Title { get; set; } = "Console PROJECT";
        
        private string input = string.Empty;
        private readonly List<string> inputHistory = PoolNew<List<string>>.Get();
        private int inputHistoryIndex;
        public Func<string, string[]> Completion = s => new[] {"test"};
        private float nextUpdate;
        
        public static int LineWidth => System.Console.BufferWidth;
        public static bool Valid => LineWidth > 0;
        
        #endregion
        
        #region Console

        public void Initialize()
        {
            System.Console.OutputEncoding = Encoding.UTF8;
        }

        public void ClearLine(int numLines)
        {
            System.Console.CursorLeft = 0;
            System.Console.Write(new string(' ', LineWidth * numLines));
            System.Console.CursorTop -= numLines;
            System.Console.CursorLeft = 0;
        }

        public void RedrawInputLine()
        {
//            if (nextUpdate - 0.449999988079071 > Interface.Controller.Now || LineWidth <= 0)
//                return;

            try
            {
                System.Console.CursorLeft = 0;
                System.Console.BackgroundColor = Log.ColorBackground;
                ClearLine(1);
                if (input.Length != 0)
                {
                    System.Console.ForegroundColor = Log.ColorInput;
                    System.Console.Write(input.Length >= LineWidth - 2
                        ? input.Substring(input.Length - (LineWidth - 2))
                        : input);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public void Update()
        {
            if (!Valid)
                return;

//            if (nextUpdate < Interface.Controller.Now)
//            {
//                RedrawInputLine();
//                nextUpdate = Interface.Controller.Now + 0.5f;
//            }

//            System.Console.Title = Title;
            try
            {
                if (!System.Console.KeyAvailable)
                    return;
            }
            catch (Exception)
            {
                return;
            }

            var key = System.Console.ReadKey();
            if (key.Key != ConsoleKey.DownArrow && key.Key != ConsoleKey.UpArrow)
                inputHistoryIndex = 0;

            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                {
                    if (input.Length <= 0)
                        break;

                    input = input.Substring(0, input.Length - 1);
                    RedrawInputLine();
                    break;
                }
                case ConsoleKey.Tab:
                {
                    var completion = Completion;
                    var arr = completion?.Invoke(input);
                    if (arr == null || arr.Length == 0)
                        break;

                    input = arr[0];
                    RedrawInputLine();
                    break;
                }
                case ConsoleKey.Enter:
                {
                    ClearLine(1);
                    System.Console.ForegroundColor = Log.ColorInput;
                    System.Console.WriteLine("> " + input);
                    inputHistory.Insert(0, input);
                    if (inputHistory.Count > 50)
                        inputHistory.RemoveRange(50, inputHistory.Count - 50);
                    
                    input = string.Empty;
                    RedrawInputLine();
                    break;
                }
                case ConsoleKey.Escape:
                {
                    input = string.Empty;
                    RedrawInputLine();
                    break;
                }
                case ConsoleKey.UpArrow:
                {
                    if (inputHistory.Count == 0)
                        break;
                    if (inputHistoryIndex < 0)
                        inputHistoryIndex = 0;
                    if (inputHistoryIndex >= inputHistory.Count - 1)
                    {
                        inputHistoryIndex = inputHistory.Count - 1;
                        input = inputHistory[inputHistoryIndex];
                        RedrawInputLine();
                        break;
                    }

                    input = inputHistory[inputHistoryIndex++];
                    RedrawInputLine();
                    break;
                }
                case ConsoleKey.DownArrow:
                {
                    if (inputHistory.Count == 0)
                        break;
                    if (inputHistoryIndex >= inputHistory.Count - 1)
                        inputHistoryIndex = inputHistory.Count - 2;

                    input = inputHistoryIndex < 0 ? string.Empty : inputHistory[inputHistoryIndex--];
                    RedrawInputLine();
                    break;
                }
                default:
                {
                    if (key.KeyChar == char.MinValue)
                        break;

                    input += key.KeyChar;
                    RedrawInputLine();
                    break;
                }
            }
        }

        #endregion
        
        #region Methods

        public void AddMessage(string message, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            ClearLine(message.Split('\n')
                .Aggregate(0, (sum, line) => sum + (int) Math.Ceiling(line.Length / (double) LineWidth)));
            System.Console.WriteLine(message);
            RedrawInputLine();
        }

        #endregion
    }
}
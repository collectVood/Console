using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    public class ConsoleManager
    {
        #region Variables

        public static string ConsoleTitle
        {
            get => System.Console.Title;
            set => System.Console.Title = value;
        }

        private float _updateFrequency = 0.25f;
        
        private string _input = string.Empty;
        private readonly List<string> _inputHistory = PoolNew<List<string>>.Get();
        private int _inputHistoryIndex;

        private int _completionIndex;
        private Func<string, string[]> _completion = s =>
        {
            // I'll rewrite this thing later maybe
            
            var data = new string[0];
            if (s.StartsWith("/"))
            {
                s = s.Remove(0, 1);
            }
            else
                return data;

            var pluginsCount = Interface.Plugins.Count;
            var currentIndex = 0;
            
            for (var i = 0; i < pluginsCount; i++)
            {
                var plugin = Interface.Plugins[i];
                foreach (var kvp in plugin.Commands)
                {
                    if (!kvp.Value.Name.StartsWith(s) && !kvp.Value.FullName.StartsWith(s))
                        continue;
                    
                    Array.Resize(ref data, data.Length + 1);
                    data[currentIndex++] = '/' + kvp.Value.Name;
                }
            }

            return data;
        };

        private Action<string> _onInput = s =>
        {
            var result = Interface.Call("ICanInput", s);
            if (result is bool b1 && !b1)
                return;
            
            result = Interface.Call("ICanLogInput", s);
            if (result is bool b2 && !b2)
            {
                Log.Input(s);
            }

            Interface.CallHook("IOnInput", s);
        };
        
        private double _nextUpdate;

        private int BufferWidth => System.Console.BufferWidth;
        private bool HasBuffer => BufferWidth > 0;
        
        #endregion
        
        #region Constructor

        internal ConsoleManager()
        {
            const string defaultTitle = "The Console";
            
            System.Console.OutputEncoding = Encoding.UTF8;
            ConsoleTitle = defaultTitle;
        }
        
        #endregion
        
        #region Console

        /// <summary>
        /// Clear lines
        /// </summary>
        /// <param name="numLines">Lines to clear</param>
        private void ClearLine(int numLines)
        {
            System.Console.CursorLeft = 0;
            System.Console.Write(new string(' ', BufferWidth * numLines));
            System.Console.CursorTop -= numLines;
            System.Console.CursorLeft = 0;
        }

        /// <summary>
        /// Redraw input line
        /// </summary>
        private void RedrawInputLine()
        {
            if (_nextUpdate - _updateFrequency > Interface.Controller.Now || BufferWidth <= 0)
                return;

            try
            {
                System.Console.CursorLeft = 0;
                System.Console.BackgroundColor = Log.ColorBackground;
                ClearLine(1);
                if (string.IsNullOrEmpty(_input)) return;
                
                System.Console.ForegroundColor = Log.ColorInput;
                System.Console.Write(_input.Length >= BufferWidth - 2
                    ? _input.Substring(_input.Length - (BufferWidth - 2))
                    : _input);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        /// <summary>
        /// Update console input, etc
        /// </summary>
        public void Update()
        {
            if (!HasBuffer)
                return;

            if (_nextUpdate < Interface.Controller.Now)
            {
                RedrawInputLine();
                _nextUpdate = Interface.Controller.Now + _updateFrequency;
            }
            
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
                _inputHistoryIndex = 0;

            if (key.Key != ConsoleKey.Tab)
                _completionIndex = 0;

            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                {
                    if (_input.Length <= 0)
                        break;

                    _input = _input.Substring(0, _input.Length - 1);
                    RedrawInputLine();
                    break;
                }
                
                case ConsoleKey.Tab:
                {
                    var arr = _completion?.Invoke(_input);
                    if (arr == null || arr.Length == 0)
                        break;

                    if (arr.Length < _completionIndex + 1)
                        _completionIndex = 0;
                    
                    _input = arr[_completionIndex++] ?? string.Empty;
                    RedrawInputLine();
                    break;
                }
                
                case ConsoleKey.Enter:
                {
                    ClearLine(1);
                    System.Console.ForegroundColor = Log.ColorInput;
                    System.Console.WriteLine("> " + _input);
                    _inputHistory.Insert(0, _input);
                    if (_inputHistory.Count > 50)
                        _inputHistory.RemoveRange(50, _inputHistory.Count - 50);

                    _onInput?.Invoke(_input);
                    _input = string.Empty;
                    RedrawInputLine();
                    break;
                }
                
                case ConsoleKey.Escape:
                {
                    _input = string.Empty;
                    RedrawInputLine();
                    break;
                }
                
                case ConsoleKey.UpArrow:
                {
                    if (_inputHistory.Count == 0)
                        break;
                    if (_inputHistoryIndex < 0)
                        _inputHistoryIndex = 0;
                    if (_inputHistoryIndex >= _inputHistory.Count - 1)
                    {
                        _inputHistoryIndex = _inputHistory.Count - 1;
                        _input = _inputHistory[_inputHistoryIndex];
                        RedrawInputLine();
                        break;
                    }

                    _input = _inputHistory[_inputHistoryIndex++];
                    RedrawInputLine();
                    break;
                }
                
                case ConsoleKey.DownArrow:
                {
                    if (_inputHistory.Count == 0)
                        break;
                    if (_inputHistoryIndex >= _inputHistory.Count - 1)
                        _inputHistoryIndex = _inputHistory.Count - 2;

                    _input = _inputHistoryIndex < 0 ? string.Empty : _inputHistory[_inputHistoryIndex--];
                    RedrawInputLine();
                    break;
                }
                
                default:
                {
                    if (key.KeyChar == char.MinValue)
                        break;

                    _input += key.KeyChar;
                    RedrawInputLine();
                    break;
                }
            }
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Add a message in console
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="color">Color of the message</param>
        public void AddMessage(string message, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            ClearLine(message.Split('\n')
                .Aggregate(0, (sum, line) => sum + (int) Math.Ceiling(line.Length / (double) BufferWidth)));
            System.Console.WriteLine(message);
            RedrawInputLine();
        }

        #endregion
    }
}
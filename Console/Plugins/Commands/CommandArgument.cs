using System;
using System.Text.RegularExpressions;

namespace Console.Plugins.Commands
{
    public class CommandArgument
    {
        #region Variables

        public Command Command { get; private set; }
        
        public string Entry { get; private set; }
        public string[] Args { get; private set; }

        public bool IsValid => Command != null && Command.Owner.IsLoaded;

        #endregion

        public bool Execute()
        {
            if (!IsValid)
                return false;
            
            Command.Execute(Args);
            return true;
        }
        
        #region Building

        public static CommandArgument Build(string entry)
        {
            var instance = PoolNew<CommandArgument>.Get();
            return instance.BuildCommand(entry);
        }

        public CommandArgument BuildCommand(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return this;

            var spaceIndex = entry.IndexOf(' ');
            var command = string.Empty;
            if (spaceIndex == -1)
            {
                if (entry.Length != 0)
                {
                    command = entry;
                }
                else
                {
                    return this;
                }
            }

            for (var i = 0; i < Interface.Plugins.Count; i++)
            {
                Command = Interface.Plugins[i].FindCommand(command);
                if (Command != null)
                    break;
            }

            if (Command == null)
            {
                return this;
            }

            var text = entry.Substring(spaceIndex + 1);
            Args = BuildArguments(text);

            return this;
        }

        // Original (not escaped): "([^"']+)"|'([^"']+)'|[^"'a-zA-Z\s]?([^"'\s]+)[^"'a-zA-Z\s]?
        public string[] BuildArguments(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return new string[0];

            var splitted = entry.Split(' ');
            return splitted;
        }
        
        #endregion
    }
}
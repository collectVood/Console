using System.Collections.Generic;

namespace Console.Plugins.Commands
{
    public class CommandArgument
    {
        #region Variables

        public Command Command { get; private set; }
        public string[] Args { get; private set; }

        public string Arguments => Args.Length == 0 ? string.Empty : string.Join(" ", Args);
        
        public bool IsValid => Command != null && Command.Owner.IsLoaded;
        
        public List<string> Replies { get; } = PoolNew<List<string>>.Get();

        #endregion

        /// <summary>
        /// Execute command with the specified arguments
        /// </summary>
        /// <returns>True if command argument instance is valid</returns>
        public bool Execute()
        {
            if (!IsValid)
                return false;
            
            Command.Execute(this);
            for (var i = 0; i < Replies.Count; i++)
            {
                Log.Info(Replies[i]);
            }
            
            return true;
        }

        /// <summary>
        /// Tests whether command argument has enough arguments
        /// </summary>
        /// <param name="amount">Amount of arguments</param>
        /// <returns>True if has <see cref="amount"/> entries</returns>
        public bool HasArgs(int amount = 1)
        {
            return Args.Length >= amount;
        }

        public void Reply(string message)
        {
            Replies.Add(message);
        }
        
        #region Building

        /// <summary>
        /// Build command argument instance from string entry
        /// </summary>
        /// <param name="entry">Input for building command argument</param>
        /// <returns>Built command argument</returns>
        public static CommandArgument Build(string entry)
        {
            var instance = PoolNew<CommandArgument>.Get();
            return instance.BuildCommand(entry);
        }

        /// <summary>
        /// Build current command argument from string entry
        /// </summary>
        /// <param name="entry">Input for building command argument</param>
        /// <returns>Current instance</returns>
        public CommandArgument BuildCommand(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return this;

            var spaceIndex = entry.IndexOf(' ');
            var command = spaceIndex == -1 ? entry : entry.Substring(0, spaceIndex);

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

            var text = spaceIndex == -1 ? string.Empty : entry.Substring(spaceIndex + 1);
            Args = BuildArguments(text);

            return this;
        }

        /// <summary>
        /// Builds arguments array from string entry
        /// </summary>
        /// <param name="entry">Input for building arguments</param>
        /// <returns>Arguments array</returns>
        public static string[] BuildArguments(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return new string[0];

            var splitted = entry.Split(' ');
            return splitted;
        }
        
        #endregion
    }
}
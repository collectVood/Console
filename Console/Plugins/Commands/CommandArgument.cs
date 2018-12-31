namespace Console.Plugins.Commands
{
    public class CommandArgument
    {
        #region Variables

        public Command Command { get; private set; }
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
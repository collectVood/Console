namespace Console.Plugins
{
    [Info("ConsoleBase", "Iv Misticos and Hamster", "1.0.0")]
    public class ConsoleBase : Plugin
    {
        [HookMethod("IOnServerInput")]
        public void IOnServerInput(string input)
        {
            if (input.StartsWith("/"))
                Interface.CallHook("IOnServerCommand", input.Substring(1));
        }

        [HookMethod("IOnServerCommand")]
        public void IOnServerCommand(string command)
        {
            Log.Debug($"Called a command {command}");
            Command cmd;
            Interface.Plugins.ForEach(x =>
            {
                foreach (var kvp in x.Commands)
                {
                    // Bla
                }
            });
        }

        [Command("test")]
        public void CommandTest(object[] args)
        {
            Log.Debug("TEST CALLED");
        }
    }
}
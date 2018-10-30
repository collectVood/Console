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
        public void IOnServerCommand(string command, params string[] args)
        {
            Log.Debug($"Called a command {command}");
        }

        [Command("test")]
        public void CommandTest()
        {
            Log.Debug("TEST CALLED");
        }
    }
}
// ReSharper disable InconsistentNaming

using Console.Plugins.Commands;
using Console.Plugins.Hooks;

namespace Console.Plugins.Core
{
    [Info("ConsoleBase", "Iv Misticos", "1.0.0")]
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
            Interface.Call("OnServerCommand", command);
            
            // TODO
        }

        [Command("test")]
        public void CommandTest()
        {
            Log.Debug("TEST CALLED");
        }
    }
}
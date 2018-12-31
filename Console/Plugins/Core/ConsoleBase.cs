// ReSharper disable InconsistentNaming

using Console.Plugins.Commands;
using Console.Plugins.Hooks;

namespace Console.Plugins.Core
{
    [Info("ConsoleBase", "Iv Misticos", "1.0.0")]
    public class ConsoleBase : Plugin
    {
        [HookMethod("IOnInput")]
        public void IOnInput(string input)
        {
            if (input.StartsWith("/"))
                Interface.CallHook("IOnCommand", input.Substring(1));
            else
                Interface.CallHook("OnInput", input);
        }

        [HookMethod("IOnCommand")]
        public void IOnCommand(string command)
        {
            Log.Debug($"Called a command {command}");
            Interface.CallHook("OnCommand", command);
        }

        [Command("test")]
        public void CommandTest()
        {
            Log.Debug("TEST CALLED");
        }
    }
}
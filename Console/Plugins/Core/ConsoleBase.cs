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
        public void IOnCommand(string entry)
        {
            var arg = CommandArgument.Build(entry);
            if (!arg.Execute())
            {
                Log.Info("Couldn't execute latest command");
                return;
            }
            
            Interface.CallHook("OnCommand", arg);
        }

        [Command("test")]
        public void CommandTest(string[] args)
        {
            var arguments = args.Length == 0 ? "Nothing" : string.Join(" ", args);
            Log.Debug("TEST CALLED");
            Log.Debug($"Arguments: {arguments}");
        }
    }
}
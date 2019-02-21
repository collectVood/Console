// ReSharper disable InconsistentNaming

using Console.Plugins.Commands;
using Console.Plugins.Hooks;

namespace Console.Plugins.Core
{
    [Info("Core", "Iv Misticos", "1.0.0")]
    public class Core : Plugin
    {
        [HookMethod("IOnInput")]
        public void IOnInput(string input)
        {
            if (input.StartsWith("/"))
                Interface.CallHook("IOnCommand", input.Substring(1));
            else
                Interface.CallHook("OnInput", input);
        }
        
        [HookMethod("ICanLogInput")]
        public object ICanLogInput(string input)
        {
            return Interface.Call("CanLogInput", input);
        }
        
        [HookMethod("ICanInput")]
        public object ICanInput(string input)
        {
            return Interface.Call("CanInput", input);
        }

        [HookMethod("IOnCommand")]
        public void IOnCommand(string entry)
        {
            var arg = CommandArgument.Build(entry);
            var result = Interface.Call("CanExecuteCommand", arg);
            if (result is bool && result.Equals(false) || !arg.Execute())
                return;
            
            Interface.CallHook("OnCommand", arg);
        }

        [Command("version")]
        public void CommandVersion(CommandArgument arg)
        {
            arg.Reply("Version Info:\n" +
                     $"Console: {Interface.Controller.Version}\n" +
                     $"Core: {Version}");
        }
    }
}
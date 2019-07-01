using Console.Plugins.Attributes;
using Console.Plugins.Commands;

namespace Console.Plugins.Core
{
    [Info("Core", "Iv Misticos", "1.0.0")]
    public class Core : Plugin
    {
        #region Hooks
        
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
        
        #endregion
        
        #region Commands

        [Command("version")]
        public void CommandVersion(CommandArgument arg)
        {
            arg.Reply("Version Info:\n" +
                     $"Console: {Interface.Controller.Version}\n" +
                     $"Core: {Version}");
        }

        [Command("load")]
        public void CommandLoad(CommandArgument arg)
        {
            if (arg.Args.Length != 1)
            {
                arg.Reply("Please, use \"core.load Name\"");
                return;
            }

            Interface.Load(arg.Args[0]);
        }

        [Command("unload")]
        public void CommandUnload(CommandArgument arg)
        {
            if (arg.Args.Length != 1)
            {
                arg.Reply("Please, use \"core.unload Name\"");
                return;
            }

            Interface.Unload(arg.Args[0]);
        }

        [Command("reload")]
        public void CommandReload(CommandArgument arg)
        {
            if (arg.Args.Length != 1)
            {
                arg.Reply("Please, use \"core.reload Name\"");
                return;
            }

            Interface.Unload(arg.Args[0]);
            Interface.Load(arg.Args[0]);
        }

        [Variable("debug")]
        public int DebugLevel
        {
            get => Interface.Controller.DebugLevel;
            set => Interface.Controller.DebugLevel = value;
        }
        
        #endregion
    }
}
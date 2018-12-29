using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Console.Plugins
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
            var cmds = PoolNew<List<Command>>.Get();
            Interface.Plugins.ForEach(x =>
            {
                foreach (var kvp in x.Commands)
                {
                    if (kvp.Value.FullName == command || kvp.Value.Name == command)
                        cmds.Add(kvp.Value);
                }
            });

            switch (cmds.Count)
            {
                case 0:
                    return;
                case 1:
                    cmds[0]?.Call(command);
                    break;
                default:
                    Log.Info("There were some commands found.");
                    break;
            }
        }

        [Command("test")]
        public void CommandTest()
        {
            Log.Debug("TEST CALLED");
        }
    }
}
namespace Console.Plugins
{
    [Info("Example Plugin", "Iv Misticos", "1.0.0")]
    [Description("Test Plugin")]
    public class ExamplePlugin : Plugin
    {
        [HookMethod("OnServerCommand")]
        public void OnServerCommand(string cmd)
        {
            Log.Info("OnServerCommand called!");
            Log.Info($"Command: {cmd}");
        } 
    }
}
namespace Console.Plugins
{
    [Info("ExamplePlugin", "Iv Misticos", "1.0.0")]
    [Description("Test Plugin")]
    public class ExamplePlugin : Plugin
    {
        [HookMethod("OnFrame")]
        public void OnFrame()
        {
            Log.Info("OnFrame called!");
        }
    }
}
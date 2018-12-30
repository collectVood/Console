namespace Console.Plugins
{
    public class HookResult
    {
        /// <summary>
        /// Plugin
        /// </summary>
        public Plugin Plugin { get; }

        /// <summary>
        /// Result (what plugin returned)
        /// </summary>
        public object Result { get; }

        /// <summary>
        /// Constructor for HookConflict
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="result">Result</param>
        public HookResult(Plugin plugin, object result)
        {
            Plugin = plugin;
            Result = result;
        }

        public override string ToString() => $"{Plugin.Title} ({Result.GetType()}: {Result})";
    }
}
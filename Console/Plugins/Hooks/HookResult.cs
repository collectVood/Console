namespace Console.Plugins.Hooks
{
    public class HookResult
    {
        /// <summary>
        /// Plugin instance
        /// </summary>
        public Plugin Plugin { get; }

        /// <summary>
        /// Returned value
        /// </summary>
        public object Value { get; }

        public HookResult(Plugin plugin, object value)
        {
            Plugin = plugin;
            Value = value;
        }

        /// <summary>
        /// Formats current hook result
        /// </summary>
        /// <returns>String representation of hook result</returns>
        public override string ToString() => $"{Plugin.Title} ({Value.GetType()}: {Value})";
    }
}
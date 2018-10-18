namespace Console.Plugins
{
    public class HookConflict
    {
        /// <summary>
        /// Plugin #1
        /// </summary>
        private Plugin P1;
        /// <summary>
        /// Plugin #2
        /// </summary>
        private Plugin P2;

        /// <summary>
        /// Result (what plugin returned) #1
        /// </summary>
        private object R1;
        /// <summary>
        /// Result (what plugin returned) #2
        /// </summary>
        private object R2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">Plugin #1</param>
        /// <param name="p2">Plugin #2</param>
        /// <param name="val1">Result #1</param>
        /// <param name="val2">Result #2</param>
        public HookConflict(Plugin p1, Plugin p2, object val1, object val2)
        {
            P1 = p1;
            P2 = p2;
            R1 = val1;
            R2 = val2;
        }

        public override string ToString() => $"{P1.Title} ({R1}) and {P2.Title} ({R2})";
    }
}
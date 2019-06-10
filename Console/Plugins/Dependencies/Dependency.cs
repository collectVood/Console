using System.Reflection;

namespace Console.Plugins.Dependencies
{
    public class Dependency
    {
        public Plugin Owner { get; }
        public FieldInfo Field { get; }
        public string Name { get; }

        public Dependency(Plugin plugin, string name, FieldInfo field)
        {
            Owner = plugin;
            Field = field;
            Name = name;
        }

        /// <summary>
        /// Update dependency field value
        /// </summary>
        public void Update()
        {
            var plugin = Interface.FindPlugin(Name);
            Field.SetValue(Owner, plugin);
        }

        /// <summary>
        /// Returns true if the field has a matching signature for dependency field
        /// </summary>
        /// <param name="field">Field Info instance for the needed field</param>
        /// <returns>True if the method has a matching signature for dependency field</returns>
        public static bool HasMatchingSignature(FieldInfo field)
        {
            return field.FieldType == typeof(Plugin);
        }
    }
}
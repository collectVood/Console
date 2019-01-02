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

        public void Update()
        {
            var plugin = Interface.FindPlugin(Name);
            Field.SetValue(Owner, plugin);
        }

        public static bool HasMatchingSignature(FieldInfo field)
        {
            return field.FieldType == typeof(Plugin);
        }
    }
}
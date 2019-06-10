using System.Reflection;

namespace Console.Plugins
{
    public class Version
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Patch;

        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public Version(Assembly assembly)
        {
            var version = assembly.GetName().Version;
            
            Major = version.Major;
            Minor = version.Minor;
            Patch = version.Build;
        }

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        public static bool operator ==(Version a, Version b) =>
            a != null && b != null && a.Major == b.Major && a.Minor == b.Minor && a.Patch == b.Patch;

        public static bool operator !=(Version a, Version b) => !(a == b); 

        public override bool Equals(object obj)
        {
            if (obj is Version ver)
                return this == ver;

            return false;
        }

        public override int GetHashCode() => ToString().GetHashCode();
    }
}
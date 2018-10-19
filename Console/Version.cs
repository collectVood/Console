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

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        public static bool operator ==(Version a, Version b) =>
            a != null && b != null && a.Major == b.Major && a.Minor == b.Minor && a.Patch == b.Patch;

        public static bool operator !=(Version a, Version b) => !(a == b);
    }
}
using System;

namespace Console.Plugins
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class InfoAttribute : Attribute
    {
        internal string Title { get; }
        internal string Author { get; }
        internal Version Version { get; }

        public InfoAttribute(string title, string author, Version version)
        {
            Title = title;
            Author = author;
            Version = version;
        }

        public InfoAttribute(string title, string author, string version)
        {
            var ver = new Version(1, 0, 0);
            var inputs = version.Split('.');
            if (inputs.Length == 3 && int.TryParse(inputs[0], out var v1) && int.TryParse(inputs[1], out var v2) &&
                int.TryParse(inputs[2], out var v3))
                ver = new Version(v1, v2, v3);


            Title = title;
            Author = author;
            Version = ver;
        }
    }
}
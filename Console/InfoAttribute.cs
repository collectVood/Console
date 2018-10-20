using System;

namespace Console.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InfoAttribute : Attribute
    {
        public string Title { get; }
        public string Author { get; }
        public Version Version { get; }

        public InfoAttribute(string Title, string Author, Version Version)
        {
            this.Title = Title;
            this.Author = Author;
            this.Version = Version;
        }

        public InfoAttribute(string Title, string Author, string Version)
        {
            var ver = new Version(1, 0, 0);
            var inputs = Version.Split('.');
            if (inputs.Length == 3 && int.TryParse(inputs[0], out var v1) && int.TryParse(inputs[1], out var v2) &&
                int.TryParse(inputs[2], out var v3))
                ver = new Version(v1, v2, v3);


            this.Title = Title;
            this.Author = Author;
            this.Version = ver;
        }
    }
}
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
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class FileEntry
    {

        [XmlAttribute]
        public string Name { get; set; }

        public string Description { get; set; }

        public string SourceLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Stack<string> items = new Stack<string>();

                items.Push(Name);
                items.Push(@"/");

                DirectoryEntry dir = Parent;
                while (true)
                {
                    items.Push(dir.Name);

                    dir = dir.Parent;
                    if (dir == null)
                    {
                        items.Pop();
                        break;
                    }
                    else
                        items.Push(@"/");
                }

                items.Pop();

                while (items.Count > 0)
                    sb.Append(items.Pop());

                return sb.ToString();
            }
        }
        public string DestinationLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Stack<string> items = new Stack<string>();

                items.Push(Name);
                items.Push(@"\");

                DirectoryEntry dir = Parent;
                while (true)
                {
                    items.Push(dir.Name);

                    dir = dir.Parent;
                    if (dir == null)
                        break;
                    else
                        items.Push(@"\");
                }

                while (items.Count > 0)
                    sb.Append(items.Pop());

                return sb.ToString();
            }
        }
        [XmlAttribute]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or Sets the parent of this File.
        /// </summary>
        [XmlIgnore]
        public DirectoryEntry Parent { get; set; }

        public FileEntry()
        { }

        public FileEntry(string name)
        {
            Name = name;
        }
    }
}

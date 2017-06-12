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

                sb.Append(Parent?.SourceLocation ?? "");
                sb.Append(Name);

                return sb.ToString();
            }
        }
        public string DestinationLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Parent?.DestinationLocation ?? "");
                sb.Append(Name);

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

using System;
using System.Collections.Generic;
using System.Linq;
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

        public string SourceLocation { get; set; }
        public string DestinationLocation { get; set; }
        [XmlAttribute]
        public string Hash { get; set; }

        public FileEntry()
        { }

        public FileEntry(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            FileEntry file = obj as FileEntry;
            if (file == null)
                return false;

            return this.Name == file.Name;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            return hash;
        }
    }
}

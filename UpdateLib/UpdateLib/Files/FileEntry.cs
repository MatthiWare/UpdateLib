using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class FileEntry : EntryBase
    {
        /// <summary>
        /// The calculated hash for the file
        /// </summary>
        [XmlAttribute]
        public string Hash { get; set; }

        public FileEntry() { }

        public FileEntry(string name)
        {
            Name = name;
        }


    }
}

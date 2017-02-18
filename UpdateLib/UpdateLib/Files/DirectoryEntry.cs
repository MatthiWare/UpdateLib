using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// Represents a directory in the update file.
    /// Directories can contain files and subdirectories.
    /// </summary>
    [Serializable]
    public class DirectoryEntry
    {
        /// <summary>
        /// Gets or sets the name of the <see cref="DirectoryEntry">directory</see>. 
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets how many files there are in this directory and its subdirectories. 
        /// </summary>
        public int Count
        {
            get
            {
                int _count = Files.Count;

                foreach(DirectoryEntry di in Directories)
                    _count += di.Count;

                return _count;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DirectoryEntry">subdirectories</see>.
        /// </summary>
        [XmlArray("Directories"), XmlArrayItem("Directory")]
        public List<DirectoryEntry> Directories { get;  set; }

        /// <summary>
        /// Gets the list of <see cref="FileEntry">files</see> in this directory.
        /// </summary>
        [XmlArray("Files"), XmlArrayItem("File")]
        public List<FileEntry> Files { get;  set; }

        /// <summary>
        /// Gets or Sets the Parent of this Directory
        /// </summary>
        [XmlIgnore]
        public DirectoryEntry Parent { get; set; }

        public DirectoryEntry()
        {
            Files = new List<FileEntry>();
            Directories = new List<DirectoryEntry>();
        }

        /// <summary>
        /// .ctor of <see cref="DirectoryEntry"/>
        /// </summary>
        public DirectoryEntry(string name)
        {
            this.Name = name;
            Files = new List<FileEntry>();
            Directories = new List<DirectoryEntry>();
        }

    }
}

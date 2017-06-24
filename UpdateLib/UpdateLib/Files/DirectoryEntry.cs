using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

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
                return Items.Count + Directories.Sum(d => d.Count);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DirectoryEntry">subdirectories</see>.
        /// </summary>
        [XmlArray("Directories"), XmlArrayItem("Directory")]
        public List<DirectoryEntry> Directories { get; set; } = new List<DirectoryEntry>();

        /// <summary>
        /// Gets the list of <see cref="EntryBase">files</see> in this directory.
        /// </summary>
        [XmlElement(typeof(FileEntry))]
        [XmlElement(typeof(RegistryKeyEntry))]
        public List<EntryBase> Items { get; set; } = new List<EntryBase>();

        /// <summary>
        /// Gets or Sets the Parent of this Directory
        /// </summary>
        [XmlIgnore]
        public DirectoryEntry Parent { get; set; }

        [XmlIgnore]
        public string SourceLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (Parent == null)
                    return string.Empty;

                sb.Append(Parent.SourceLocation);
                sb.Append(Name);
                sb.Append(@"/");

                return sb.ToString();
            }
        }

        [XmlIgnore]
        public string DestinationLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Parent?.DestinationLocation ?? string.Empty);
                sb.Append(Name);
                sb.Append(@"\");

                return sb.ToString();
            }
        }

        /// <summary>
        /// .ctor of <see cref="DirectoryEntry"/>
        /// </summary>
        public DirectoryEntry() { }

        /// <summary>
        /// .ctor of <see cref="DirectoryEntry"/>
        /// </summary>
        public DirectoryEntry(string name)
            : this()
        {
            Name = name;
        }

        public void Add(DirectoryEntry folder)
        {
            folder.Parent = folder;
            Directories.Add(folder);
        }

        public void Add(EntryBase file)
        {
            file.Parent = this;
            Items.Add(file);
        }

        public void Remove(DirectoryEntry folder)
        {
            folder.Parent = null;
            Directories.Remove(folder);
        }

        public void Remove(EntryBase file)
        {
            file.Parent = null;
            Items.Remove(file);
        }
    }
}

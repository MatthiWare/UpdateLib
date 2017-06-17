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
                return Files.Count + Directories.Select(d => d.Count).Sum();
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DirectoryEntry">subdirectories</see>.
        /// </summary>
        [XmlArray("Directories"), XmlArrayItem("Directory")]
        public List<DirectoryEntry> Directories { get; set; } = new List<DirectoryEntry>();

        /// <summary>
        /// Gets the list of <see cref="FileEntry">files</see> in this directory.
        /// </summary>
        [XmlArray("Files"), XmlArrayItem("File")]
        public List<FileEntry> Files { get; set; } = new List<FileEntry>();

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
                    return "";

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

                sb.Append(Parent?.DestinationLocation ?? "");
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
        {
            Name = name;
        }

        public void Add(DirectoryEntry folder)
        {
            folder.Parent = folder;
            Directories.Add(folder);
        }

        public void Add(FileEntry file)
        {
            file.Parent = this;
            Files.Add(file);
        }

        public void Remove(DirectoryEntry folder)
        {
            folder.Parent = this;
            Directories.Remove(folder);
        }

        public void Remove(FileEntry file)
        {
            file.Parent = this;
            Files.Remove(file);
        }
    }
}

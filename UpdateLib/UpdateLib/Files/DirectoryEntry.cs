using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public List<DirectoryEntry> Directories { get; private set; }
        /// <summary>
        /// Gets the list of <see cref="FileEntry">files</see> in this directory.
        /// </summary>
        public List<FileEntry> Files { get; private set; }

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

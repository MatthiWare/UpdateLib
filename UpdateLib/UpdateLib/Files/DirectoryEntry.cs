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
        public DirectoryEntry()
        {
            Files = new List<FileEntry>();
            Directories = new List<DirectoryEntry>();
        }

    }
}

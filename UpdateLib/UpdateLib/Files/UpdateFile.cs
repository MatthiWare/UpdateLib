using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// The UpdateFile 
    /// </summary>
    [Serializable]
    public class UpdateFile
    {
        /// <summary>
        /// Gets or sets the version of the current  update.
        /// The versionstring should be parsable by the <see cref="Version"/> to be valid. 
        /// </summary>
        public string VersionString { get; set; }

        /// <summary>
        /// Gets the root folder of the application
        /// </summary>
        public DirectoryEntry ApplicationDirectory { get; private set; }
        /// <summary>
        /// Gets the root folder for other files related elsewhere on the filesystem
        /// </summary>
        public DirectoryEntry OtherDirectory { get; private set; }

        /// <summary>
        /// Gets the count of all the files in the <see cref="ApplicationDirectory"/>, <see cref="OtherDirectory"/>
        /// and their subdirectories.
        /// </summary>
        public int Count { get { return ApplicationDirectory.Count + OtherDirectory.Count; } }

        public UpdateFile()
        {
        }

        public override bool Equals(object obj)
        {
            UpdateFile file = obj as UpdateFile;
            if (file == null)
                return false;

            if (!file.VersionString.Equals(this.VersionString))
                return false;
            


            return true;

        }

        public override int GetHashCode()
        {
            int hash = 7;

            hash = (hash * 7) + VersionString.GetHashCode();
            
            return hash;
        }
    }
}

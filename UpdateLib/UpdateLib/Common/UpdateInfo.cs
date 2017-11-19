using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Common
{
    [Serializable]
    public class UpdateInfo
    {
        /// <summary>
        /// Gets or sets the version of the current  update.
        /// The versionstring should be parsable by the <see cref="System.Version"/> to be valid. 
        /// </summary>
        public UpdateVersion Version { get; set; } = new UpdateVersion(1);

        /// <summary>
        /// Gets the folders of the project
        /// </summary>
        [XmlArray("Folders"), XmlArrayItem("Directory")]
        public List<DirectoryEntry> Folders { get; private set; } = new List<DirectoryEntry>();

        /// <summary>
        /// Gets the count of all the files in the <see cref="Folders"/>
        /// and their subdirectories.
        /// </summary>
        [XmlIgnore]
        public int FileCount { get { return Folders.Select(d => d.Count).Sum(); } }

        [XmlIgnore]
        public int RegistryKeyCount { get { return Registry.Select(r => r.Count).Sum(); } }

        [XmlArray("Registry"), XmlArrayItem("Directory")]
        public List<DirectoryEntry> Registry { get; private set; } = new List<DirectoryEntry>();

        public UpdateInfo()
        {

        }
    }
}

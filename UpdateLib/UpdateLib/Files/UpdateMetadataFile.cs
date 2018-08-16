using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable, Description("Update Metadata File")]
    public class UpdateMetadataFile : FileBase<UpdateMetadataFile>
    {

        public const string FILE_NAME = "update-metadata.gz";

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
        public int FileCount => Folders.Sum(dir => dir.Count);

        [XmlIgnore]
        public int RegistryKeyCount => Registry.Sum(reg => reg.Count);

        [XmlArray("Registry"), XmlArrayItem("Directory")]
        public List<DirectoryEntry> Registry { get; private set; } = new List<DirectoryEntry>();

        public UpdateMetadataFile() { }

        public override UpdateMetadataFile Load() => throw new NotImplementedException();

        public override UpdateMetadataFile Load(Stream stream)
           => base.Load(stream);

        public override void Save() => throw new NotImplementedException();

        public override void Save(Stream stream)
            => base.Save(stream);
    }
}

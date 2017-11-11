/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// The UpdateFile 
    /// </summary>
    [Serializable]
    public class UpdateFile
    {
        /// <summary>
        /// Gets or sets the name of the application
        /// </summary>
        [XmlAttribute]
        public string ApplicationName { get; set; } = "UpdateLib";

        /// <summary>
        /// Gets or sets the version of the current  update.
        /// The versionstring should be parsable by the <see cref="System.Version"/> to be valid. 
        /// </summary>
        [XmlAttribute]
        public VersionXml Version { get; set; } = new Version("1.0.0.0");

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

        public UpdateFile()
        {
        }

        /// <summary>
        /// Saves the current <see cref="UpdateFile"/> to the output <see cref="Stream"/>
        /// </summary>
        /// <param name="output">The output <see cref="Stream"/> to write the object to</param>
        public void Save(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!output.CanWrite)
                throw new ArgumentException("Stream is not writable", nameof(output));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            // ns.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(typeof(UpdateFile), string.Empty);
            serializer.Serialize(output, this, ns);
        }

        /// <summary>
        /// Saves the current <see cref="UpdateFile"/> to a specified file.
        /// This method will delete the file specified in the path parameter if it exists and recreates it.
        /// </summary>
        /// <param name="path">The path of the file where to save</param>
        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo file = new FileInfo(path);

            if (file.Exists)
                file.Delete();

            using (var stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write))
                Save(stream);
        }

        /// <summary>
        /// Loads a <see cref="UpdateFile"/> from a input <see cref="Stream"/>.
        /// This method doesn't close/dispose the <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">The input that contains this object</param>
        /// <returns>The loaded instance of <see cref="UpdateFile"/></returns>
        public static UpdateFile Load(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (!input.CanRead)
                throw new ArgumentException("Stream is not readable", nameof(input));

            XmlSerializer serializer = new XmlSerializer(typeof(UpdateFile));

            XmlReader xml = new XmlTextReader(input);

            if (!serializer.CanDeserialize(xml))
                throw new InvalidOperationException("The current stream cannot be deserialized");

            UpdateFile file = (UpdateFile)serializer.Deserialize(xml);

            new UpdateFileProcessorTask(file).ConfigureAwait(false).Start().AwaitTask();

            return file;
        }

        /// <summary>
        /// Loads a <see cref="UpdateFile"/> from a path. 
        /// </summary>
        /// <param name="path">The path to the <see cref="UpdateFile"/> save file</param>
        /// <returns>The loaded instance of <see cref="UpdateFile"/></returns>
        public static UpdateFile Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            FileInfo file = new FileInfo(path);

            if (!file.Exists)
                throw new FileNotFoundException("The UpdateFile doesn't exist.", path);

            using (Stream stream = file.OpenRead())
                return Load(stream);
        }
    }
}

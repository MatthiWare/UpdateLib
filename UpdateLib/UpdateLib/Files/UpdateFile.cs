using MatthiWare.UpdateLib.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
        /// The versionstring should be parsable by the <see cref="Version"/> to be valid. 
        /// </summary>
        [XmlAttribute]
        public string VersionString { get; set; } = "1.0.0.0";

        /// <summary>
        /// Gets the root folder of the application
        /// </summary>
        public DirectoryEntry ApplicationDirectory { get; set; } = new DirectoryEntry("%appdir%");
        /// <summary>
        /// Gets the root folder for other files related elsewhere on the filesystem
        /// </summary>
        public DirectoryEntry OtherDirectory { get; set; } = new DirectoryEntry("%otherdir%");

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

        /// <summary>
        /// Saves the current <see cref="UpdateFile"/> to the output <see cref="Stream"/>
        /// </summary>
        /// <param name="output">The output <see cref="Stream"/> to write the object to</param>
        public void Save(Stream output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            if (!output.CanWrite)
                throw new ArgumentException("Stream is not writable", "output");

            XmlSerializer serializer = new XmlSerializer(typeof(UpdateFile));
            serializer.Serialize(output, this);
        }

        /// <summary>
        /// Saves the current <see cref="UpdateFile"/> to a specified file.
        /// This method will delete the file specified in the path parameter if it exists and recreates it.
        /// </summary>
        /// <param name="path">The path of the file where to save</param>
        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (File.Exists(path))
                File.Delete(path);

            using (Stream s = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                Save(s);
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
                throw new ArgumentNullException("input");

            if (!input.CanRead)
                throw new ArgumentException("Stream is not readable", "input");

            XmlSerializer xml = new XmlSerializer(typeof(UpdateFile));

            UpdateFile file = (UpdateFile)xml.Deserialize(input);

            UpdateFileProcessorTask processor = new UpdateFileProcessorTask(file);
            processor.Start();

            processor.AwaitTask();

            return file;
        }

        /// <summary>
        /// Loads a <see cref="UpdateFile"/> from a path. 
        /// </summary>
        /// <param name="path">The path to the <see cref="UpdateFile"/> save file</param>
        /// <returns>The loaded instance of <see cref="UpdateFile"/></returns>
        public static UpdateFile Load(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (!File.Exists(path))
                throw new FileNotFoundException("The UpdateFile doesn't exist.", path);

            using (Stream s = File.Open(path, FileMode.Open, FileAccess.Read))
                return Load(s);
        }
    }
}

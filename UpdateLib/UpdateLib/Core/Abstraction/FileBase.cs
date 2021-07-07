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

using System;
using System.IO;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Common.Abstraction
{
    /// <summary>
    /// The base class for all the files that need to be able to save/load from disk. 
    /// </summary>
    /// <typeparam name="T">Serializable file class</typeparam>
    [Serializable]
    public abstract class FileBase<T> where T : new()
    {
        /// <summary>
        /// Default ctor needed for serialization
        /// </summary>
        protected FileBase() { }

        /// <summary>
        /// Saves the file using the default path
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Saves the file using a specified path
        /// </summary>
        /// <param name="path">The path to save the file to</param>
        public virtual void Save(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var fi = new FileInfo(path);

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (var stream = fi.Open(FileMode.Create, FileAccess.Write))
                Save(stream);
        }

        /// <summary>
        /// Saves the file using a specified stream
        /// </summary>
        /// <param name="stream">The stream to save the file to.</param>
        public virtual void Save(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new ArgumentException("Unwritable stream", nameof(stream));

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, this);
        }

        /// <summary>
        /// Loads the file from the default path
        /// </summary>
        /// <returns>A loaded instance of the <see cref="T"/> file.</returns>
        public abstract T Load();

        /// <summary>
        /// Loads the file from a specified path
        /// </summary>
        /// <param name="path">The path to load the file from</param>
        /// <returns>A loaded instance of the <see cref="T"/> file.</returns>
        public virtual T Load(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var fi = new FileInfo(path);

            if (!fi.Exists) throw new FileNotFoundException("File does not exist", path);

            using (var stream = fi.Open(FileMode.Open, FileAccess.Read))
                return Load(stream);
        }

        /// <summary>
        /// Loads the file from a specified stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A loaded instance of the <see cref="T"/> file.</returns>
        public virtual T Load(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Unreadable stream", nameof(stream));

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }
    }
}

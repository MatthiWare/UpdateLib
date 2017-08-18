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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using System.Diagnostics;

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
        [DebuggerDisplay("{DestinationLocation}")]
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
            if (folder == null) throw new ArgumentNullException(nameof(folder));

            folder.Parent = this;
            Directories.Add(folder);
        }

        public void Add(EntryBase file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            file.Parent = this;
            Items.Add(file);
        }

        public bool Remove(DirectoryEntry folder)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));

            folder.Parent = null;
            return Directories.Remove(folder);
        }

        public bool Remove(EntryBase file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            file.Parent = null;
            return Items.Remove(file);
        }

        /// <summary>
        /// Gets all the items including the items of childs
        /// </summary>
        /// <returns>A list of items</returns>
        public IEnumerable<EntryBase> GetItems()
        {
            return Items.Concat(Directories.SelectMany(d => d.GetItems()));
        }
    }
}

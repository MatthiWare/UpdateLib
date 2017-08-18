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
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public abstract class EntryBase
    {
        public EntryBase()
        {

        }

        [XmlAttribute]
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets the parent of this File.
        /// </summary>
        [XmlIgnore]
        public DirectoryEntry Parent { get; set; }

        public string SourceLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Parent?.SourceLocation ?? string.Empty);
                sb.Append(Name);

                return sb.ToString();
            }
        }

        public string DestinationLocation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Parent?.DestinationLocation ?? string.Empty);
                sb.Append(Name);

                return sb.ToString();
            }
        }
    }
}

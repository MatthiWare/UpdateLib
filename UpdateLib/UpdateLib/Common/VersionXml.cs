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
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Common
{
    /// <summary>
    /// Serialize System.Version to XML: https://stackoverflow.com/a/18962224/6058174
    /// </summary>
    [Serializable]
    [XmlType("Version")]
    [DebuggerDisplay("Version")]
    public class VersionXml
    {
        public VersionXml() { }

        public VersionXml(Version version)
        {
            Version = version;
        }

        [XmlIgnore]
        public Version Version { get; set; } = null;

        [XmlText]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string Value
        {
            get { return Version?.ToString() ?? string.Empty; }
            set { Version = new Version(value); }
        }

        public static implicit operator Version(VersionXml VersionXml) => VersionXml.Version;

        public static implicit operator VersionXml(Version Version) => new VersionXml(Version);

        public override string ToString() => Value;
    }
}

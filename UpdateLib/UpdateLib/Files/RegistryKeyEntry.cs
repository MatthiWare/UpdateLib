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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    [DebuggerDisplay("RegistryKeyEntry: {DestinationLocation}")]
    public class RegistryKeyEntry : EntryBase
    {
        /// <summary>
        /// The type of registry key  
        /// </summary>
        [XmlAttribute]
        public RegistryValueKind Type { get; set; }

        /// <summary>
        /// The value of the key
        /// </summary>
        public object Value { get; set; } = "Test";

        public RegistryKeyEntry()
            : this(string.Empty, RegistryValueKind.String, null)
        { }

        public RegistryKeyEntry(string name, RegistryValueKind type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}

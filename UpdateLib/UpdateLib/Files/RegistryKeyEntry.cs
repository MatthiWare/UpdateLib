using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
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
        public object Value { get; set; }

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

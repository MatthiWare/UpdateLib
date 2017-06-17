﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class RegistryKeyEntry
    {

        public RegistryValueKind Type { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Parent?.FullName ?? string.Empty);
                sb.Append(Name);

                return sb.ToString();
            }
        }

        [XmlIgnore]
        public RegistryFolderEntry Parent { get; set; }

    }
}

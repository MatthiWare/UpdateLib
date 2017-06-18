using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class RegistryDirectoryEntry
    {
        [XmlIgnore]
        public bool IsRoot { get { return Parent == null; } }

        [XmlIgnore]
        public RegistryDirectoryEntry Parent { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public int Count
        {
            get
            {
                return Keys.Count + Directories.Select(x => x.Count).Sum();
            }
        }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (IsRoot)
                    return string.Empty;

                sb.Append(Parent.FullName);
                sb.Append(Name);
                sb.Append("\\");

                return sb.ToString();
            }
        }

        public RegistryDirectoryEntry() { }
        public RegistryDirectoryEntry(string name)
        {
            Name = name;
        }

        [XmlArray("Directories"), XmlArrayItem("Directory")]
        public List<RegistryDirectoryEntry> Directories { get; set; } = new List<RegistryDirectoryEntry>();

        [XmlArray("Keys"), XmlArrayItem("Key")]
        public List<RegistryKeyEntry> Keys { get; set; } = new List<RegistryKeyEntry>();

        public void Add(RegistryDirectoryEntry folder)
        {
            folder.Parent = folder;
            Directories.Add(folder);
        }

        public void Add(RegistryKeyEntry key)
        {
            key.Parent = this;
            Keys.Add(key);
        }

        public void Remove(RegistryDirectoryEntry folder)
        {
            folder.Parent = this;
            Directories.Remove(folder);
        }

        public void Remove(RegistryKeyEntry key)
        {
            key.Parent = this;
            Keys.Remove(key);
        }
    }
}

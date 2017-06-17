using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class RegistryFolderEntry
    {
        [XmlIgnore]
        public bool IsRoot { get { return Parent == null; } }

        [XmlIgnore]
        public RegistryFolderEntry Parent { get; set; }

        public string Name { get; set; }

        public int Count
        {
            get
            {
                return Keys.Count + Folders.Select(x => x.Count).Sum();
            }
        }

        public string FullName
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (IsRoot)
                    return "";

                sb.Append(Parent.FullName);
                sb.Append(Name);
                sb.Append("\\");

                return sb.ToString();
            }
        }

        public List<RegistryFolderEntry> Folders { get; private set; } = new List<RegistryFolderEntry>();

        public List<RegistryKeyEntry> Keys { get; private set; } = new List<RegistryKeyEntry>();
    }
}

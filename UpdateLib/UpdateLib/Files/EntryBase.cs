using System;
using System.Collections.Generic;
using System.Linq;
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

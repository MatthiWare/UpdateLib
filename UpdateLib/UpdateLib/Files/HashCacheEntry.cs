using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class HashCacheEntry
    {
        [XmlAttribute]
        public string Hash { get; set; }

        [XmlElement("Path")]
        public string FilePath { get;  set; }

        [XmlAttribute("Time")]
        public long Ticks { get;  set; }

        public HashCacheEntry() { }

        public HashCacheEntry(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            FilePath = Path.GetFullPath(file);

            if (!File.Exists(FilePath))
                throw new FileNotFoundException("Unable to find file", FilePath);

            Hash = HashUtil.GetHash(FilePath);

            Ticks = File.GetLastWriteTime(FilePath).Ticks;
        }

        public void Recalculate(long tick)
        {
            if (tick != Ticks)
                Hash = HashUtil.GetHash(FilePath);
        }

    }
}

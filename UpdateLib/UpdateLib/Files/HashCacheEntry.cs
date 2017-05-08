using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Security;
using System;
using System.IO;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class HashCacheEntry
    {
        [XmlAttribute]
        public string Hash { get; set; }

        [XmlElement("Path")]
        public string FilePath { get; set; }

        [XmlAttribute("Time")]
        public long Ticks { get; set; }

        public HashCacheEntry() { }

        public HashCacheEntry(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            FilePath = Path.GetFullPath(file);

            if (!File.Exists(FilePath))
                throw new FileNotFoundException("Unable to find file", FilePath);

            Hash = HashUtil.GetHash(FilePath);

            Ticks = File.GetLastWriteTime(FilePath).Ticks;
        }

        public void Recalculate(long tick)
        {
            try
            {
                if (tick != Ticks)
                {
                    Hash = HashUtil.GetHash(FilePath);
                    Ticks = tick;

                    Logger.Debug(GetType().Name, $"Recalculated Time: {DateTime.FromBinary(Ticks).ToString()} Name: {FilePath} Hash: {Hash}");
                }
            }
            catch (Exception ex) // file might no longer exist or is in use
            {
                Hash = string.Empty;
                Ticks = -1;

                Logger.Error(GetType().Name, ex);
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class HashCacheFile
    {
        [XmlArray("Items")]
        [XmlArrayItem("Entry")]
        public List<HashCacheEntry> Items { get; set; }
        
        private static string storagePath;
        
        public HashCacheFile()
        {
            Items = new List<HashCacheEntry>();
        }

        #region Save/Load
        private static string GetStoragePath()
        {
            if (string.IsNullOrEmpty(storagePath))
                storagePath = string.Format(
                    @"{0}\{1}\MatthiWare.UpdateLib\Cache\HashCacheFile.xml",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Assembly.GetEntryAssembly().GetName().Name);

            return storagePath;
        }

        public static HashCacheFile Load()
        {
            if (!File.Exists(GetStoragePath()))
                return null;

            using (Stream stream = File.Open(GetStoragePath(), FileMode.Open, FileAccess.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                return (HashCacheFile)serializer.Deserialize(stream);
            }
        }

        public void Save()
        {
            FileInfo fi = new FileInfo(GetStoragePath());

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (Stream stream = fi.Open(FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                serializer.Serialize(stream, this);
            }
        }
        #endregion

    }
}

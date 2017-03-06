using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string name = Assembly.GetEntryAssembly().GetName().Name;
                storagePath = $@"{appdata}\{name}\MatthiWare.UpdateLib\Cache\HashCacheFile.xml";
            }

            return storagePath;
        }

        public static HashCacheFile Load()
        {
            if (!File.Exists(GetStoragePath()))
                return null;

            using (Stream stream = File.Open(GetStoragePath(), FileMode.Open, FileAccess.Read))
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

            if (fi.Exists)
                fi.Delete();


            using (Stream stream = fi.Open(FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                serializer.Serialize(stream, this);
            }
        }
        #endregion

    }
}

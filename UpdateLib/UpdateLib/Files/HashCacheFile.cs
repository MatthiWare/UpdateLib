using MatthiWare.UpdateLib.Utils;
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
        public const string CACHE_FOLDER_NAME = "Cache";
        public const string FILE_NAME = "HashCacheFile.xml";

        [XmlArray("Items")]
        [XmlArrayItem("Entry")]
        public List<HashCacheEntry> Items { get; set; }

        private static Lazy<string> storagePath = new Lazy<string>(GetStoragePath);

        public HashCacheFile()
        {
            Items = new List<HashCacheEntry>();
        }

        #region Save/Load
        private static string GetStoragePath()
        {
            string path = IOUtils.GetAppDataPath();
            string productName = Updater.ProductName;
            string name = Assembly.GetEntryAssembly().GetName().Name;

            return $@"{path}\{name}\{productName}\{CACHE_FOLDER_NAME}\{FILE_NAME}";
        }

        public static HashCacheFile Load()
        {
            if (!File.Exists(storagePath.Value))
                return null;

            using (Stream stream = File.Open(storagePath.Value, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                return (HashCacheFile)serializer.Deserialize(stream);
            }
        }

        public void Save()
        {
            FileInfo fi = new FileInfo(storagePath.Value);

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

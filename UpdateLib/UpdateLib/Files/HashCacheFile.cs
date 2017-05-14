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
                string path = GetPathPrefix();
                string productName = GetProductName();
                string name = Assembly.GetEntryAssembly().GetName().Name;

                storagePath = $@"{path}\{name}\{productName}\{CACHE_FOLDER_NAME}\{FILE_NAME}";
            }

            return storagePath;
        }

        private static string GetProductName()
        {
            AssemblyProductAttribute attr = Attribute.GetCustomAttribute(Assembly.GetAssembly(typeof(HashCacheFile)), typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
            return attr?.Product ?? "";
        }

        private static string GetPathPrefix()
        {
            switch (Updater.Instance.InstallationMode)
            {
                case InstallationMode.Local:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                default:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
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

using MatthiWare.UpdateLib.Security;
using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq;

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

        private readonly object sync = new object();

        public HashCacheFile()
        {
            Items = new List<HashCacheEntry>();
        }

        public void AddOrUpdateEntry(string fullPath, string hash = "")
        {
            lock (sync)
            {
                long ticks = File.GetLastWriteTime(fullPath).Ticks;
                hash = string.IsNullOrEmpty(hash) ? HashUtil.GetHash(fullPath) : hash;

                HashCacheEntry entry = Items.FirstOrDefault(f => f.FilePath == fullPath);

                if (entry == null)
                {
                    entry = new HashCacheEntry();
                    entry.FilePath = fullPath;
                    entry.Hash = hash;
                    entry.Ticks = ticks;

                    Items.Add(entry);
                }
                else
                {
                    entry.Ticks = ticks;
                    entry.Hash = hash;
                }

                Updater.Instance.Logger.Debug(nameof(HashCacheFile), nameof(AddOrUpdateEntry), $"Cache updated for file -> '{entry.FilePath}'");
            }
        }

        #region Save/Load
        private static string GetStoragePath()
        {
            string path = IOUtils.AppDataPath;

            return $@"{path}\{CACHE_FOLDER_NAME}\{FILE_NAME}";
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

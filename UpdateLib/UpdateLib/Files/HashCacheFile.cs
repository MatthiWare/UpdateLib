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

        /// <summary>
        /// Adds or updates an entry in the file.
        /// </summary>
        /// <param name="fullPath">The full path to the file</param>
        /// <param name="hash">[Optional]: If the hash has already been calculated you can pass it in here so we don't need to recalculate it.</param>
        public void AddOrUpdateEntry(string fullPath, string hash = "")
        {
            lock (sync)
            {
                HashCacheEntry entry = Items.FirstOrDefault(f => f.FilePath == fullPath);

                if (entry == null)
                {
                    entry = new HashCacheEntry(fullPath);
                    Items.Add(entry);
                }
                else
                    entry.Recalculate();

                Updater.Instance.Logger.Debug(nameof(HashCacheFile), nameof(AddOrUpdateEntry), $"Cache updated for file -> '{entry.FilePath}'");
            }
        }

        #region Save/Load
        private static string GetStoragePath()
        {
            string path = IOUtils.AppDataPath;

            return $@"{path}\{CACHE_FOLDER_NAME}\{FILE_NAME}";
        }

        /// <summary>
        /// Loads the <see cref="HashCacheFile"/> from the default storage location 
        /// </summary>
        /// <returns>The loaded <see cref="HashCacheFile"/> or null if it doesn't exist </returns>
        public static HashCacheFile Load()
        {
            return Load(GetStoragePath());
        }

        /// <summary>
        /// Loads the <see cref="HashCacheFile"/> from the given storage location 
        /// </summary>
        /// <param name="path">The storage location</param>
        /// <returns>The loaded <see cref="HashCacheFile"/> or null if it doesn't exist </returns>
        public static HashCacheFile Load(string path)
        {
            if (!File.Exists(path))
                return null;

            using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                return (HashCacheFile)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Saves the <see cref="HashCacheFile"/> in the default storage location 
        /// </summary>
        public void Save()
        {
            Save(GetStoragePath());
        }

        /// <summary>
        /// Saves the <see cref="HashCacheFile"/> in the given storage location 
        /// </summary>
        /// <param name="path">The storage location</param>
        public void Save(string path)
        {
            FileInfo fi = new FileInfo(path);

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

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
    public class HashCacheFile : IList<HashCacheEntry>
    {
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

        #region IList interface implementation
        public int Count { get { return Items.Count; } }

        public bool IsReadOnly { get { return false; } }

        public HashCacheEntry this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public int IndexOf(HashCacheEntry item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, HashCacheEntry item)
        {
            Items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        public void Add(HashCacheEntry item)
        {
            Items.Add(item);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(HashCacheEntry item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(HashCacheEntry[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(HashCacheEntry item)
        {
           return Items.Remove(item);
        }

        public IEnumerator<HashCacheEntry> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        #endregion

    }
}

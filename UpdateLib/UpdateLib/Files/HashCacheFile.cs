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
        private List<HashCacheEntry> items;
        
        private static string storagePath;
        
        public HashCacheFile()
        {
            items = new List<HashCacheEntry>();
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

            using (Stream stream = File.Open(GetStoragePath(), FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                return (HashCacheFile)serializer.Deserialize(stream);
            }
        }

        public void Save()
        {
            using (Stream stream = File.Open(GetStoragePath(), FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HashCacheFile));
                serializer.Serialize(stream, this);
            }
        }
        #endregion

        #region IList interface implementation
        public int Count { get { return items.Count; } }

        public bool IsReadOnly { get { return false; } }

        public HashCacheEntry this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public int IndexOf(HashCacheEntry item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, HashCacheEntry item)
        {
            items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public void Add(HashCacheEntry item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(HashCacheEntry item)
        {
            return items.Contains(item);
        }

        public void CopyTo(HashCacheEntry[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(HashCacheEntry item)
        {
           return items.Remove(item);
        }

        public IEnumerator<HashCacheEntry> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
        #endregion

    }
}

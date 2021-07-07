/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class HashCacheFile : FileBase<HashCacheFile>
    {
        public const string FILE_NAME = "Cache.xml";

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

                entry?.Recalculate();

                if (entry == null)
                {
                    entry = new HashCacheEntry(fullPath);
                    Items.Add(entry);
                }

                //Updater.Instance.Logger.Debug(nameof(HashCacheFile), nameof(AddOrUpdateEntry), $"Cache updated for file -> '{entry.FilePath}'");
            }
        }

        #region Save/Load
        private static string GetStoragePath() => $@"{IOUtils.CachePath}\{FILE_NAME}";

        /// <summary>
        /// Loads the <see cref="HashCacheFile"/> from the default storage location 
        /// </summary>
        /// <returns>The loaded <see cref="HashCacheFile"/> or null if it doesn't exist </returns>
        public override HashCacheFile Load()
            => Load(GetStoragePath());

        /// <summary>
        /// Saves the <see cref="HashCacheFile"/> in the default storage location 
        /// </summary>
        public override void Save()
            => Save(GetStoragePath());

        #endregion

    }
}

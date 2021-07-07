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
using System.IO;
using System.Xml.Serialization;

using MatthiWare.UpdateLib.Security;

namespace MatthiWare.UpdateLib.Common
{
    [Serializable]
    public class HashCacheEntry
    {
        [XmlAttribute]
        public string Hash { get; set; }

        [XmlElement("Path")]
        public string FilePath { get; set; }

        [XmlAttribute("Time")]
        public long Ticks { get; set; } = -1;

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

        public void Recalculate(string hash = "")
        {
            try
            {
                var tick = File.GetLastWriteTime(FilePath).Ticks;

                if (Ticks != -1 && tick == Ticks) return;

                Hash = string.IsNullOrEmpty(hash) ? HashUtil.GetHash(FilePath) : hash;
                Ticks = tick;

                //Updater.Instance.Logger.Debug(nameof(HashCacheEntry), nameof(Recalculate), $"Recalculated Time: {DateTime.FromBinary(Ticks).ToString()} Name: {FilePath} Hash: {Hash}");
            }
            catch (Exception ex) // file might no longer exist or is in use
            {
                Hash = string.Empty;
                Ticks = -1;

                //Updater.Instance.Logger.Error(nameof(HashCacheEntry), nameof(Recalculate), ex);
            }
        }

    }
}

using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class HashCacheEntry
    {
        public string Hash { get; private set; }

        public string FilePath { get; private set; }

        public long Ticks { get; private set; }

        public HashCacheEntry(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            FilePath = Path.GetFullPath(file);

            if (!File.Exists(FilePath))
                throw new FileNotFoundException("Unable to find file", FilePath);

            Hash = HashUtil.GetHash(FilePath);

            Ticks = File.GetLastWriteTime(FilePath).Ticks;
        }

    }
}

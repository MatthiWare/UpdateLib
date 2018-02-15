using System;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class CacheFile : FileBase<CacheFile>
    {
        private static Lazy<string> m_filePath = new Lazy<string>(() => $"{IOUtils.AppDataPath}\\Cache.xml");

        public UpdateVersion CurrentVersion { get; set; }

        public override CacheFile Load()
            => Load(m_filePath);

        public override void Save()
            => Save(m_filePath);
    }
}

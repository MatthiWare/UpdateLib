using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatedFilesTask : AsyncTask<bool>
    {
        private UpdateFile m_updateFile;
        private HashCacheFile m_cacheFile;
        private PathVariableConverter m_converter;

        public CheckForUpdatedFilesTask(UpdateFile update, HashCacheFile cache, PathVariableConverter converter)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            m_updateFile = update;
            m_cacheFile = cache;
            m_converter = converter;
        }

        protected override void DoWork()
        {
            foreach (DirectoryEntry dir in m_updateFile.Folders)
                Enqueue(new Action<DirectoryEntry>(RecursiveCheck), dir);

            AwaitWorkers();

            Result = m_updateFile.FileCount > 0 || m_updateFile.RegistryKeyCount > 0;
        }

        private void RecursiveCheck(DirectoryEntry dir)
        {
            dir.Items.RemoveAll(fe =>
            {
                string convertedPath = m_converter.Replace(fe.DestinationLocation);
                HashCacheEntry cacheEntry = m_cacheFile.Items.Find(hash => hash.FilePath.Equals(convertedPath));

                if (cacheEntry == null)
                    return false;

                bool val = (fe as FileEntry).Hash.Equals(cacheEntry.Hash);
                return val;
            });

            IEnumerable<DirectoryEntry> dirsToCheck = dir.Directories.Where(d => d.Count > 0);
            int left = dirsToCheck.Count();

            foreach (DirectoryEntry de in dir.Directories)
            {
                if (--left == 0)
                    RecursiveCheck(de);
                else
                    Enqueue(new Action<DirectoryEntry>(RecursiveCheck), de);
            }
        }
    }
}

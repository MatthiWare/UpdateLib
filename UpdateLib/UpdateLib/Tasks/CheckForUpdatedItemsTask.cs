using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatedItemsTask : AsyncTask<bool>
    {
        private UpdateFile m_updateFile;
        private HashCacheFile m_cacheFile;

        public CheckForUpdatedItemsTask(UpdateFile update, HashCacheFile cache)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            if (cache == null) throw new ArgumentNullException(nameof(cache));

            m_updateFile = update;
            m_cacheFile = cache;
        }

        protected override void DoWork()
        {
            foreach (DirectoryEntry dir in m_updateFile.Folders)
                Enqueue(new Action<DirectoryEntry>(CheckFiles), dir);

            foreach (DirectoryEntry dir in m_updateFile.Registry)
                Enqueue(new Action<DirectoryEntry>(CheckRegister), dir);

            AwaitWorkers();

            Result = m_updateFile.FileCount > 0 || m_updateFile.RegistryKeyCount > 0;
        }

        private void CheckFiles(DirectoryEntry dir)
        {
            dir.Items.RemoveAll(fe =>
            {
                string convertedPath = Updater.Instance.Converter.Convert(fe.DestinationLocation);
                HashCacheEntry cacheEntry = m_cacheFile.Items.Find(hash => hash.FilePath.Equals(convertedPath));

                if (cacheEntry == null)
                    return false;

                bool val = (fe as FileEntry).Hash.Equals(cacheEntry.Hash);
                return val;
            });

            IEnumerable<DirectoryEntry> dirsToCheck = dir.Directories.Where(d => d.Count > 0);
            int left = dirsToCheck.Count();

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                if (--left == 0)
                    CheckFiles(subDir);
                else
                    Enqueue(new Action<DirectoryEntry>(CheckFiles), subDir);
            }
        }

        private void CheckRegister(DirectoryEntry dir)
        {
            dir.Items.RemoveAll(entry =>
            {
                RegistryKeyEntry key = entry as RegistryKeyEntry;

                if (key == null)
                    return true;

                return RegistryHelper.IsSame(key);
            });

            IEnumerable<DirectoryEntry> dirsToCheck = dir.Directories.Where(d => d.Count > 0);
            int left = dirsToCheck.Count();

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                if (--left == 0)
                    CheckRegister(subDir);
                else
                    Enqueue(new Action<DirectoryEntry>(CheckRegister), dir);
            }
        }
    }
}

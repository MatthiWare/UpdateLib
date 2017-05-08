using MatthiWare.UpdateLib.Files;
using System;

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
            Action<DirectoryEntry> call = new Action<DirectoryEntry>(RecursiveCheck);

            foreach (DirectoryEntry dir in m_updateFile.Folders)
                Enqueue(call, dir);

            AwaitWorkers();

            Result = m_updateFile.Count > 0;
        }

        private void RecursiveCheck(DirectoryEntry dir)
        {
            dir.Files.RemoveAll(fe =>
            {
                string convertedPath = m_converter.Replace(fe.DestinationLocation);
                HashCacheEntry cacheEntry = m_cacheFile.Items.Find(hash => hash.FilePath.Equals(convertedPath));

                if (cacheEntry == null)
                    return false;

                bool val = fe.Hash.Equals(cacheEntry.Hash);
                return val;
            });

            Action<DirectoryEntry> call = new Action<DirectoryEntry>(RecursiveCheck);

            foreach (DirectoryEntry de in dir.Directories)
                Enqueue(call, de);
        }
    }
}

using MatthiWare.UpdateLib.Files;
using System;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatedFilesTask : AsyncTaskBase<bool>
    {
        private UpdateFile updateFile;
        private HashCacheFile cacheFile;
        private PathVariableConverter converter;

        public CheckForUpdatedFilesTask(UpdateFile update, HashCacheFile cache, PathVariableConverter converter)
        {
            updateFile = update;
            cacheFile = cache;
            this.converter = converter;
        }

        private void RecursiveCheck(DirectoryEntry dir)
        {
            dir.Files.RemoveAll(fe =>
            {
                string convertedPath = converter.Replace(fe.DestinationLocation);
                HashCacheEntry cacheEntry = cacheFile.Items.Find(hash => hash.FilePath.Equals(convertedPath));

                if (cacheEntry == null)
                    return false;

                bool val = fe.Hash.Equals(cacheEntry.Hash);
                return val;
            });

            Action<DirectoryEntry> call = new Action<DirectoryEntry>(RecursiveCheck);

            foreach (DirectoryEntry de in dir.Directories)
                Enqueue(call, de);
        }

        protected override void DoWork()
        {
            Action<DirectoryEntry> call = new Action<DirectoryEntry>(RecursiveCheck);

            Enqueue(call, updateFile.ApplicationDirectory);

            Enqueue(call, updateFile.OtherDirectory);

            AwaitWorkers();

            Result = updateFile.Count > 0;
        }
    }
}

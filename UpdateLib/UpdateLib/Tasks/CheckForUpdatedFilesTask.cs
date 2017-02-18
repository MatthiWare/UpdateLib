using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatedFilesTask
    {
        private UpdateFile updateFile;
        private HashCacheFile cacheFile;
        private PathVariableConverter converter;

        private Queue<WaitHandle> whQueue;
        private readonly object sync = new object();

        public CheckForUpdatedFilesTask(UpdateFile update, HashCacheFile cache, PathVariableConverter converter)
        {
            updateFile = update;
            cacheFile = cache;
            this.converter = converter;

            whQueue = new Queue<WaitHandle>();
        }

        public void Start()
        {
            Action<DirectoryEntry> call = new Action<DirectoryEntry>(RecursiveCheck);

            lock (sync)
                whQueue.Enqueue(call.BeginInvoke(updateFile.ApplicationDirectory, new AsyncCallback(r => call.EndInvoke(r)), null).AsyncWaitHandle);

            lock (sync)
                whQueue.Enqueue(call.BeginInvoke(updateFile.OtherDirectory, new AsyncCallback(r => call.EndInvoke(r)), null).AsyncWaitHandle);
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

            
            foreach (DirectoryEntry de in dir.Directories)
            {
                Action<DirectoryEntry> call = new Action<DirectoryEntry>(RecursiveCheck);
                lock (sync)
                    whQueue.Enqueue(call.BeginInvoke(de, new AsyncCallback(r => call.EndInvoke(r)), null).AsyncWaitHandle);
            }
        }

        public bool AwaitTask()
        {
            while (whQueue.Count > 0)
            {
                WaitHandle wh = null;
                lock (sync)
                    wh = whQueue.Dequeue();

                wh.WaitOne();
                wh.Close();
            }

            Console.WriteLine("[INFO]: CheckForUpdatesTask {0} files ready to update.", updateFile.Count);
            return updateFile.Count > 0;
        }

    }
}

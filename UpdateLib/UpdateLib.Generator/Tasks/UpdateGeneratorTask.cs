using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Threading;
using System.IO;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Generator.Data.FilesPage;
using System.Collections.Generic;
using MatthiWare.UpdateLib.Generator.UI.Pages;
using MatthiWare.UpdateLib.Logging;

namespace MatthiWare.UpdateLib.Generator.Tasks
{
    public class UpdateGeneratorTask : AsyncTask<UpdateFile>
    {
        private delegate void AddDirRecursiveDelegate(GenFolder dir, DirectoryEntry entry);

        private GenFolder baseDir;

        private int total;
        private int done = 0;

        private InformationPage infoPage;

        public UpdateGeneratorTask(GenFolder dir, InformationPage info)
        {
            if (dir == null)
                throw new ArgumentNullException("dir", "The directory cannot be null");

            Result = new UpdateFile();

            baseDir = dir;

            total = dir.Count;

            infoPage = info;
        }

        protected override void DoWork()
        {
            foreach (GenFolder subfolder in baseDir.Directories)
            {
                DirectoryEntry entry = new DirectoryEntry(string.IsNullOrEmpty(subfolder.PathVariable) ? subfolder.Name : subfolder.PathVariable);

                Result.Folders.Add(entry);

                AddDirRecursive(subfolder, entry);
            }


            Result.ApplicationName = infoPage.ApplicationName;
            Result.VersionString = infoPage.ApplicationVersion;
        }

        private void AddDirRecursive(GenFolder dir, DirectoryEntry entry)
        {
           // Logger.Debug(GetType().Name, $"Thread: {Thread.CurrentThread.ManagedThreadId}");

            List<GenFile> files = dir.Files;
            foreach (GenFile genFile in files)
            {
                FileInfo fi = genFile.FileInfo;
                FileEntry newEntry = new FileEntry(fi.Name);
                newEntry.Hash = HashUtil.GetHash(fi.FullName);

                entry.Files.Add(newEntry);

                Interlocked.Increment(ref done);
            }

            if (files.Count > 0)
                OnTaskProgressChanged(done, total);

            foreach (GenFolder newDir in dir.Directories)
            {
                if (newDir.Count == 0)
                    continue;

                DirectoryEntry newEntry = new DirectoryEntry(string.IsNullOrEmpty(newDir.PathVariable) ? newDir.Name : newDir.PathVariable);
                entry.Directories.Add(newEntry);

                AddDirRecursiveDelegate caller = new AddDirRecursiveDelegate(AddDirRecursive);
                Enqueue(caller, newDir, newEntry);

                //AddDirRecursive(newDir, newEntry);
            }
        }
    }
}

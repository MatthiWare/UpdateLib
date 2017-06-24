using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Linq;
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

        private IList<GenFolder> registryFolders;

        public UpdateGeneratorTask(GenFolder dir, InformationPage info, IList<GenFolder> registry)
        {
            if (dir == null)
                throw new ArgumentNullException("dir", "The directory cannot be null");

            Result = new UpdateFile();

            baseDir = dir;
            registryFolders = registry;

            total = dir.Count + registry.Sum(g => g.Count);

            infoPage = info;
        }

        protected override void DoWork()
        {
            foreach (GenFolder subfolder in baseDir.Directories)
            {
                if (subfolder.Count == 0)
                    return;

                DirectoryEntry entry = new DirectoryEntry(string.IsNullOrEmpty(subfolder.PathVariable) ? subfolder.Name : subfolder.PathVariable);

                Result.Folders.Add(entry);

                AddDirRecursive(subfolder, entry);
            }

            Enqueue(new Action(AddRegistryItems), null);

            Result.ApplicationName = infoPage.ApplicationName;
            Result.VersionString = infoPage.ApplicationVersion;
        }

        private void AddRegistryItems()
        {
            foreach (GenFolder registry in registryFolders)
            {
                if (registry.Count == 0)
                    continue;

                DirectoryEntry dir = new DirectoryEntry(registry.Name);

                Result.Registry.Add(dir);

                AddRegistryRecursive(registry, dir);
            }
        }

        private void AddRegistryRecursive(GenFolder dir, DirectoryEntry entry)
        {
            List<IGenItem> keys = dir.Items;
            foreach (GenReg key in keys)
            {
                entry.Items.Add(new RegistryKeyEntry(key.Name, key.Type, key.Value));

                Interlocked.Increment(ref done);
            }

            if (keys.Count > 0)
                OnTaskProgressChanged(done, total);

            IEnumerable<GenFolder> dirsLeft = dir.Directories.Where(g => g.Count > 0);
            int left = dirsLeft.Count();

            foreach (GenFolder subDir in dirsLeft)
            {
                DirectoryEntry dirEntry = new DirectoryEntry(subDir.Name);
                entry.Directories.Add(dirEntry);

                left--;

                if (left == 0)
                    AddRegistryRecursive(subDir, dirEntry);
                else
                    Enqueue(new Action<GenFolder, DirectoryEntry>(AddRegistryRecursive), subDir, dirEntry);
            }

        }

        private void AddDirRecursive(GenFolder dir, DirectoryEntry entry)
        {
            List<IGenItem> files = dir.Items;
            foreach (GenFile genFile in files)
            {
                FileInfo fi = genFile.FileInfo;
                FileEntry newEntry = new FileEntry(fi.Name);
                newEntry.Hash = HashUtil.GetHash(fi.FullName);

                entry.Items.Add(newEntry);

                Interlocked.Increment(ref done);
            }

            if (files.Count > 0)
                OnTaskProgressChanged(done, total);

            IEnumerable<GenFolder> dirsLeft = dir.Directories.Where(g => g.Count > 0);
            int left = dirsLeft.Count();

            foreach (GenFolder subDir in dirsLeft)
            {
                DirectoryEntry dirEntry = new DirectoryEntry(string.IsNullOrEmpty(subDir.PathVariable) ? subDir.Name : subDir.PathVariable);
                entry.Directories.Add(dirEntry);

                left--;

                if (left == 0)
                    AddDirRecursive(subDir, dirEntry);
                else
                    Enqueue(new Action<GenFolder, DirectoryEntry>(AddDirRecursive), subDir, dirEntry);
            }
        }
    }
}

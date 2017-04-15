using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Threading;
using System.IO;
using MatthiWare.UpdateLib.Tasks;

namespace MatthiWare.UpdateLib.Generator.Tasks
{
    public class UpdateGeneratorTask : AsyncTaskBase<UpdateFile>
    {
        private delegate void AddDirRecursiveDelegate(DirectoryInfo dir, DirectoryEntry entry);

        private DirectoryInfo baseDir;

        private int total;
        private int done = 0;

        public UpdateGeneratorTask(DirectoryInfo dir)
        {
            if (dir == null)
                throw new ArgumentNullException("dir", "The directory cannot be null");

            if (!dir.Exists)
                throw new DirectoryNotFoundException(string.Format("The directory '{0}' does not exist.", dir.FullName));

            Result = new UpdateFile();

            baseDir = dir;

            total = dir.GetFiles("*", SearchOption.AllDirectories).Length;
        }

        protected override void DoWork()
        {
            AddDirRecursive(baseDir, Result.ApplicationDirectory);
        }

        private void AddDirRecursive(DirectoryInfo dir, DirectoryEntry entry)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo fi in files)
            {
                FileEntry newEntry = new FileEntry(fi.Name);
                newEntry.Hash = HashUtil.GetHash(fi.FullName);

                entry.Files.Add(newEntry);

                Interlocked.Increment(ref done);
            }

            if (files.Length > 0)
                OnTaskProgressChanged(done, total);

            foreach (DirectoryInfo newDir in dir.GetDirectories())
            {
                if (newDir.GetFiles("*", SearchOption.AllDirectories).Length == 0)
                    continue;

                DirectoryEntry newEntry = new DirectoryEntry(newDir.Name);
                entry.Directories.Add(newEntry);

                AddDirRecursiveDelegate caller = new AddDirRecursiveDelegate(AddDirRecursive);
                Enqueue(caller,newDir, newEntry);
            }
        }
    }
}

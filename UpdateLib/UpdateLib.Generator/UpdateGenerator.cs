using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UpdateLib.Generator
{
    public class UpdateGenerator
    {
        private UpdateFile updateFile;
        private Queue<WaitHandle> waitQueue;
        private readonly object sync = new object();

        private delegate void AddDirRecursiveDelegate(DirectoryInfo dir, DirectoryEntry entry);

        public UpdateGenerator()
        {
            updateFile = new UpdateFile();
            waitQueue = new Queue<WaitHandle>();
        }

        public void AddDirectory(DirectoryInfo dir)
        {
            if (dir == null)
                throw new ArgumentNullException("dir", "The directory cannot be null");

            if (!dir.Exists)
                throw new DirectoryNotFoundException(string.Format("The directory '{0}' does not exist.", dir.FullName));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            AddDirRecursive(dir, updateFile.ApplicationDirectory);

            while (waitQueue.Count > 0)
            {
                WaitHandle wh = null;

                lock (sync)
                    wh = waitQueue.Dequeue();

                wh.WaitOne();
            }

            sw.Stop();

            Console.WriteLine("The generation has taken: {0}ms", sw.ElapsedMilliseconds);
        }

        private void AddDirRecursive(DirectoryInfo dir, DirectoryEntry entry)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                //Console.WriteLine("Adding: {0}", fi.FullName);

                FileEntry newEntry = new FileEntry(fi.Name);
                newEntry.Hash = HashUtil.GetHash(fi.FullName);

                entry.Files.Add(newEntry);
            }


            foreach (DirectoryInfo newDir in dir.GetDirectories())
            {
                DirectoryEntry newEntry = new DirectoryEntry(newDir.Name);
                entry.Directories.Add(newEntry);

                AddDirRecursiveDelegate call = new AddDirRecursiveDelegate(AddDirRecursive);
                lock (sync)
                    waitQueue.Enqueue(call.BeginInvoke(newDir, newEntry, null, null).AsyncWaitHandle);
            }
        }


        public UpdateFile Build()
        {
            return updateFile;
        }

    }
}

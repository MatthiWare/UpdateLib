using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateCacheTask
    {

        private Queue<WaitHandle> whQueue;
        private HashCacheFile file;
        private readonly object sync = new object();

        public UpdateCacheTask()
        {
            whQueue = new Queue<WaitHandle>();
        }

        public void Start()
        {
            Action caller = new Action(Worker);
            lock (sync)
                whQueue.Enqueue(caller.BeginInvoke(new AsyncCallback(r => caller.EndInvoke(r)), null).AsyncWaitHandle);
        }

        private void Worker()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // first of lets load the file
            file = HashCacheFile.Load();

            DirectoryInfo dir = new DirectoryInfo(".");
            IEnumerable<FileInfo> files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => !f.FullName.Contains(".old.tmp"));

            Console.WriteLine("[INFO]: UpdateCacheFile found {0} files to recheck.", files.Count());

            if (file == null) // The file doesn't exist yet
            {
                Console.WriteLine("[INFO]: UpdateCacheFile doesn't exist. Creating..");

                foreach (FileInfo f in files)
                    file.Items.Add(new HashCacheEntry(f.FullName));

                file.Save();

                sw.Stop();
                Console.WriteLine("[INFO]: UpdateCacheTask took {0}ms.", sw.ElapsedMilliseconds);

                return;
            }

            foreach (FileInfo f in files)
            {
                HashCacheEntry entry = file.Items.Find(match => match.FilePath == f.FullName);
                if (entry == null)
                {
                    file.Items.Add(new HashCacheEntry(f.FullName));
                    continue;
                }

                // check to see if the file has been modified since last cache check
                entry.Recalculate(f.LastWriteTime.Ticks);
            }

            file.Save();

            sw.Stop();
            Console.WriteLine("[INFO]: UpdateCacheTask took {0}ms.", sw.ElapsedMilliseconds);

        }

        public HashCacheFile AwaitTask()
        {
            while(whQueue.Count > 0)
            {
                WaitHandle wh = null;

                lock (sync)
                    wh = whQueue.Dequeue();

                wh.WaitOne();
                wh.Close();
            }

            return file;
        }

    }
}

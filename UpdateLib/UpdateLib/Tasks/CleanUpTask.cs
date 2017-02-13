using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CleanUpTask
    {

        private Queue<WaitHandle> whQueue;
        private readonly object sync = new object();

        public string PathToClean { get; set; }
        public string SearchPattern { get; set; }
        public bool IncludeSubDirectories { get; set; }

        public CleanUpTask(string pathToCleanUp, string searchPattern = "*.old.tmp", bool includeSubDirs = true)
        {
            whQueue = new Queue<WaitHandle>();

            PathToClean = pathToCleanUp;
            SearchPattern = searchPattern;
            IncludeSubDirectories = includeSubDirs;
        }

        public void Start()
        {
            Action caller = new Action(Worker);
            whQueue.Enqueue(caller.BeginInvoke(new AsyncCallback(r => caller.EndInvoke(r)), null).AsyncWaitHandle);
        }

        private void Worker()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            DirectoryInfo dir = new DirectoryInfo(PathToClean);
            FileInfo[] files = dir.GetFiles(SearchPattern, IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            sw.Reset();

            Console.WriteLine("[INFO]: Get files to update took {0}ms.", sw.ElapsedMilliseconds);

            sw.Start();

            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR]: Unable to delete file {0} -> {1}.", file.FullName, e.Message);
                }
            }

            sw.Stop();

            Console.WriteLine("[INFO]: Deleting files took {0}ms.", sw.ElapsedMilliseconds);
        }

        public void AwaitTask()
        {
            while (whQueue.Count > 0)
            {
                WaitHandle wh = null;
                lock (sync)
                    wh = whQueue.Dequeue();

                wh.WaitOne();
                wh.Close();
            }
        }
    }
}

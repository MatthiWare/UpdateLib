using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Files
{
    public class UpdateFilePostProcessor
    {
        private UpdateFile file;
        private Queue<WaitHandle> waitQueue;
        private readonly object sync = new object();

        public UpdateFilePostProcessor(UpdateFile file)
        {
            this.file = file;
            waitQueue = new Queue<WaitHandle>();
        }

        public void PostProcess()
        {
            PostProcessDirectory(file.ApplicationDirectory);
            PostProcessDirectory(file.OtherDirectory);

            while (waitQueue.Count > 0)
            {
                WaitHandle wh = null;
                lock (sync)
                    wh = waitQueue.Dequeue();

                wh.WaitOne();
                wh.Close();
            }
        }

        private delegate void PostProcessDirectoryDelegate(DirectoryEntry entry);

        private void PostProcessDirectory(DirectoryEntry dir)
        {
            foreach (FileEntry file in dir.Files)
                file.Parent = dir;

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                subDir.Parent = dir;

                PostProcessDirectoryDelegate call = new PostProcessDirectoryDelegate(PostProcessDirectory);

                lock (sync)
                    waitQueue.Enqueue(call.BeginInvoke(subDir, new AsyncCallback(r => call.EndInvoke(r)), null).AsyncWaitHandle);
            }
        }

    }
}

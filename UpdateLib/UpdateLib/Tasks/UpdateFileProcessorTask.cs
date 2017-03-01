using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateFileProcessorTask : AsyncTaskBase
    {
        private UpdateFile file;

        public UpdateFileProcessorTask(UpdateFile file)
        {
            this.file = file;
        }


        private void PostProcessDirectory(DirectoryEntry dir)
        {
            Action<DirectoryEntry> call = new Action<DirectoryEntry>(PostProcessDirectory);

            foreach (FileEntry file in dir.Files)
                file.Parent = dir;

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                subDir.Parent = dir;
                Enqueue(call.BeginInvoke(subDir, new AsyncCallback(r => call.EndInvoke(r)), null).AsyncWaitHandle);
            }
        }

        protected override void DoWork()
        {
            PostProcessDirectory(file.ApplicationDirectory);
            PostProcessDirectory(file.OtherDirectory);

            AwaitWorkers();
        }
    }
}

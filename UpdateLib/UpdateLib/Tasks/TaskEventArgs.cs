using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Tasks
{
    public class TaskEventArgs : EventArgs
    {
        public int ProgressPercentage { get; private set; }

        public TaskEventArgs(int progress)
            :base()
        {
            ProgressPercentage = progress;
        }

    }
}

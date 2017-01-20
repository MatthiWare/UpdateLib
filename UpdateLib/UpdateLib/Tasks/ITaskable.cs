using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Tasks
{
    public interface ITaskable : IDisposable
    {
        event EventHandler<TaskEventArgs> TaskProgressChanged;
        event EventHandler<TaskEventArgs> TaskCompleted;

        bool HasError { get; }
        Exception Error { get; }

        void Execute();

    }
}

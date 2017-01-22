using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public interface IWizardPage
    {
        event EventHandler PageUpdate;
        UpdaterForm UpdaterForm { get; }
        void Cancel();
        void Execute();
        UserControl Conent { get; }
        bool NeedsCancel { get; }
        bool NeedsExecution { get; }
        bool IsBusy { get; }
        bool IsDone { get; }
        string Title { get; }
    }
}

﻿using System;
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
        bool IsBusy { get; set; }
        bool IsDone { get; set; }
        string Title { get; }
        void PageEntered();
        void Rollback();
        bool HasErrors { get; set; }
        bool NeedsRollBack { get; }
        void UpdateState();
    }
}

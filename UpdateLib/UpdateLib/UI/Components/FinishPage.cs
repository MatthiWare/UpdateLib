using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class FinishPage : UserControl, IWizardPage
    {
        public FinishPage(UpdaterForm parent)
        {
            InitializeComponent();

            _updaterForm = parent;
        }

        public UserControl Conent
        {
            get
            {
                return this;
            }
        }

        public bool IsBusy
        {
            get
            {
                return false;
            }
        }

        public bool IsDone
        {
            get
            {
                return true;
            }
        }

        public bool NeedsCancel
        {
            get
            {
                return false;
            }
        }

        public bool NeedsExecution
        {
            get
            {
                return false;
            }
        }

        public string Title
        {
            get
            {
                return txtFinished.Text;
            }
        }

        private UpdaterForm _updaterForm;
        public UpdaterForm UpdaterForm
        {
            get
            {
                return _updaterForm;
            }
        }

        public event EventHandler PageUpdate;

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        private void cbRestart_CheckedChanged(object sender, EventArgs e)
        {
            UpdaterForm.NeedsRestart = cbRestart.Checked;
        }
    }
}

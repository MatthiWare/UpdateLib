using System;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class FinishPage : UserControl, IWizardPage
    {
        public FinishPage(UpdaterForm parent)
        {
            InitializeComponent();

            _updaterForm = parent;

            txtDescription.Text = txtDescription.Text.Replace("%AppName%", parent.updateInfoFile.ApplicationName);
            txtDescription.Text = txtDescription.Text.Replace("%version%", parent.updateInfoFile.VersionString);
        }

        public UserControl Conent
        {
            get
            {
                return this;
            }
        }

        private bool _isbusy;
        public bool IsBusy
        {
            get
            {
                return _isbusy;
            }

            set
            {
                _isbusy = value;
            }
        }

        private bool _isdone;
        public bool IsDone
        {
            get
            {
                return _isdone;
            }

            set
            {
                _isdone = value;
            }
        }

        public void PageEntered()
        {
            IsDone = true;
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

        public bool NeedsRollBack { get { return false; } }

        public bool HasErrors
        {
            get; set;
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

        public void Rollback()
        {

        }
    }
}

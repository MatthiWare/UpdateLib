using System;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class IntroPage : UserControl, IWizardPage
    {
        public IntroPage(UpdaterForm parent)
        {
            InitializeComponent();

            _updateForm = parent;

            txtDesc.Text = txtDesc.Text.Replace("%AppName%", parent.updateInfoFile.ApplicationName);
            txtWelcome.Text = txtWelcome.Text.Replace("%AppName%", parent.updateInfoFile.ApplicationName);
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
                return txtWelcome.Text;
            }
        }

        private UpdaterForm _updateForm;
        public UpdaterForm UpdaterForm
        {
            get
            {
                return _updateForm;
            }
        }

        public bool HasErrors
        {
            get; set;
        }

        public event EventHandler PageUpdate;

        public bool NeedsRollBack { get { return false; } }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {

        }
    }
}

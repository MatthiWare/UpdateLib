using System;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class ChangelogPage : UserControl, IWizardPage
    {
        public ChangelogPage(UpdaterForm parent)
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
                return txtTitle.Text;
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
        
        public bool IsDone
        {get;set;
        }

        public bool HasErrors
        {
            get; set;
        }

        public void PageEntered()
        {
            IsDone = true;
        }

        public event EventHandler PageUpdate;

        public bool NeedsRollBack { get { return false; } }

        public void UpdateState()
        {

        }

        public void Cancel()
        {
            IsDone = true;
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

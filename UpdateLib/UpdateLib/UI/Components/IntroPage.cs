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
    public partial class IntroPage : UserControl, IWizardPage
    {
        public IntroPage(UpdaterForm parent)
        {
            InitializeComponent();

            _updateForm = parent;
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

        public event EventHandler PageUpdate;

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}

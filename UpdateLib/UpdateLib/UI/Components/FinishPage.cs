﻿/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Files;
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

        public void UpdateState()
        {
            if (_updaterForm.hasHadErrors)
            {
                cbRestart.Checked = false;
                cbRestart.Enabled = false;

                UpdateFile file = _updaterForm.updateInfoFile;

                txtDescription.Text = $"{file.ApplicationName} was unable to update to version {file.VersionString}!\n\n" +
                    "Check the log files for more information!\n\n" +
                    "Press Finish to close this wizard.";

                txtFinished.Text = "Finished (with errors)";
            }
            else if (_updaterForm.UserCancelled)
            {
                cbRestart.Checked = false;
                cbRestart.Enabled = false;

                UpdateFile file = _updaterForm.updateInfoFile;

                txtDescription.Text = $"{file.ApplicationName} was unable to update to version {file.VersionString}!\n\n" +
                    "Update process cancelled by the user.\n\n" +
                    "Press Finish to close this wizard.";

                txtFinished.Text = "Finished (cancelled)";
            }
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
            IsDone = true;
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

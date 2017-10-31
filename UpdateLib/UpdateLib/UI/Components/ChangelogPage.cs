/*  UpdateLib - .Net auto update library
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

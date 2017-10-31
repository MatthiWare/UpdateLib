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

using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI
{
    public partial class InputDialog : Form
    {
        public string Header
        {
            get { return this.lblHeader.Text; }
            set { this.lblHeader.Text = value; }
        }

        public string Input
        {
            get { return this.txtInput.Text; }
            set { this.txtInput.Text = value; }
        }

        public InputDialog()
        {
            InitializeComponent();
        }

        public InputDialog(string title, string header, MessageBoxButtons buttons = MessageBoxButtons.YesNo)
            : this()
        {
            Header = header;
            Text = title;

            SetUpButtons(buttons);

            txtInput.Focus();
        }

        private void SetUpButtons(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                default:
                    SetUpButton(btn3, "OK", DialogResult.OK, true);
                    break;
                case MessageBoxButtons.OKCancel:
                    SetUpButton(btn2, "OK", DialogResult.OK, true);
                    SetUpButton(btn3, "Cancel", DialogResult.Cancel);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    SetUpButton(btn3, "Ignore", DialogResult.Ignore);
                    SetUpButton(btn2, "Retry", DialogResult.Retry);
                    SetUpButton(btn1, "Abort", DialogResult.Abort, true);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    SetUpButton(btn3, "Cancel", DialogResult.Cancel);
                    SetUpButton(btn2, "No", DialogResult.No);
                    SetUpButton(btn1, "Yes", DialogResult.Yes, true);
                    break;
                case MessageBoxButtons.YesNo:
                    SetUpButton(btn3, "No", DialogResult.No);
                    SetUpButton(btn2, "Yes", DialogResult.Yes, true);
                    break;
                case MessageBoxButtons.RetryCancel:
                    SetUpButton(btn3, "Cancel", DialogResult.Cancel);
                    SetUpButton(btn2, "Retry", DialogResult.Retry, true);
                    break;
            }
        }

        private void SetUpButton(Button button, string text, DialogResult result, bool defaultButton = false)
        {
            button.Text = text;
            button.DialogResult = result;
            button.Visible = true;

            if (defaultButton)
                button.TabIndex = 0;
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (btn1.Visible)
                    btn1.PerformClick();
                else if (btn2.Visible)
                    btn2.PerformClick();
                else if (btn3.Visible)
                    btn3.PerformClick();
            }
        }
    }
}

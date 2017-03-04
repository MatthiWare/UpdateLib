using System.Drawing;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public partial class MessageDialog : Form
    {
        public string Header
        {
            get { return this.lblHeader.Text; }
            set { this.lblHeader.Text = value; }
        }

        public string Description
        {
            get { return this.lblDesc.Text; }
            set { this.lblDesc.Text = value; }
        }

        public Icon DialogIcon
        {
            set { this.pbIcon.BackgroundImage = value.ToBitmap(); }
        }

        public MessageDialog()
        {
            InitializeComponent();
        }

        public MessageDialog(string title, string header, string desc, Icon icon, MessageBoxButtons buttons = MessageBoxButtons.YesNo)
            : this()
        {
            Header = header;
            Description = desc;
            Text = title;
            DialogIcon = icon;

            SetUpButtons(buttons);
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
    }
}

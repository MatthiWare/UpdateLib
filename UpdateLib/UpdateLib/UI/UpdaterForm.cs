using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Properties;
using MatthiWare.UpdateLib.UI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public partial class UpdaterForm : Form
    {
        internal UpdateInfoFile updateInfoFile;

        private WizardPageCollection pages;

        private ButtonState stateNext = ButtonState.Next;
        private ButtonState stateCancel = ButtonState.Cancel;

        public UpdaterForm(UpdateInfoFile updateFile)
        {
            InitializeComponent();

            updateInfoFile = updateFile;

            pages = new WizardPageCollection();
            AddPage(new IntroPage(this));
            AddPage(new ChangelogPage(this));
            AddPage(new UpdatePage(this));
            AddPage(new FinishPage(this));

            SetContentPage(pages.FirstPage);

        }

        private void SetContentPage(IWizardPage page)
        {
            for (int i = pnlContent.Controls.Count - 1; i >= 0; i--)
            {
                IWizardPage item = pnlContent.Controls[i] as IWizardPage;
                if (item == null)
                    continue;

                pnlContent.Controls.RemoveAt(i);
            }

            pnlContent.Controls.Add(page.Conent);
        }

        private void AddPage(IWizardPage page)
        {
            page.PageUpdate += Page_PageUpdate;
            pages.Add(page);
        }

        private void Page_PageUpdate(object sender, EventArgs e)
        {
            IWizardPage page = (IWizardPage)sender;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            IWizardPage page = pages.Previous();
            if (page == null)
                return;

            if (page == pages.FirstPage)
                btnPrevious.Enabled = false;

            SetContentPage(page);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (pages.CurrentPage.NeedsExecution)
            {
                pages.CurrentPage.Execute();
                return;
            }

            IWizardPage page = pages.Next();
            if (page == null)
                return;

            if (!btnPrevious.Enabled)
                btnPrevious.Enabled = true;

            if (page.NeedsExecution)
                btnNext.Text = "Update";

            SetContentPage(page);

        }
    }

    public enum ButtonState
    {
        Next,
        Execute,
        Cancel,
        Finish
    }
}

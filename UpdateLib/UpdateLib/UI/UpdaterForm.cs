using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.UI.Components;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public partial class UpdaterForm : Form
    {
        internal UpdateFile updateInfoFile;
        internal bool NeedsRestart = true;
        internal bool hasHadErrors = false;
        internal bool UserCancelled = false;

        private WizardPageCollection pages;

        public UpdaterForm(UpdateFile updateFile)
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
            page.PageEntered();

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
            OnPageUpdate(page);
        }

        delegate void _OnPageUpdate(IWizardPage page);
        private void OnPageUpdate(IWizardPage page)
        {
            this.InvokeOnUI(() =>
            {
                if (page.IsDone && !page.IsBusy)
                {
                    btnNext.Enabled = true;
                    if (page == pages.CurrentPage)
                        btnNext.Focus();
                    if (page.NeedsExecution)
                        btnNext.Text = "Next >";
                }

                if (page.HasErrors && page.NeedsRollBack)
                {
                    hasHadErrors = true;
                    btnNext.Enabled = true;
                    btnPrevious.Enabled = false;
                    btnCancel.Enabled = false;
                    btnNext.Text = "Rollback";
                }

                if (!pages.CurrentPage.HasErrors && pages.CurrentPage.NeedsRollBack && hasHadErrors)
                {
                    foreach (IWizardPage wp in pages)
                        wp.UpdateState();
                }

                if (pages.AllDone())
                    btnCancel.Enabled = false;

            });
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            IWizardPage currentPage = pages.CurrentPage;
            IWizardPage page = pages.Previous();

            if (page == null)
                return;

            if (!btnNext.Enabled)
                btnNext.Enabled = true;

            if (page.NeedsExecution)
                btnNext.Text = "Next >";

            if (page == pages.FirstPage)
                btnPrevious.Enabled = false;

            SetContentPage(page);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (pages.CurrentPage.HasErrors && pages.CurrentPage.NeedsRollBack)
            {
                btnNext.Enabled = false;
                pages.CurrentPage.Rollback();
                return;
            }

            if (pages.CurrentPage.NeedsExecution && !pages.CurrentPage.IsDone)
            {
                pages.CurrentPage.Execute();
                btnNext.Enabled = false;
                return;
            }

            if (pages.CurrentPage == pages.LastPage && pages.CurrentPage.IsDone)
            {
                ExitUpdater();
                return;
            }

            IWizardPage page = pages.Next();
            if (page == null)
                return;

            if (!btnPrevious.Enabled)
                btnPrevious.Enabled = true;

            if (page.NeedsExecution && !page.IsDone)
                btnNext.Text = "Update";

            if (page.NeedsExecution && !page.IsDone && page.IsBusy)
                btnNext.Enabled = false;

            if (page.HasErrors && page.NeedsRollBack)
                btnNext.Text = "Rollback";

            if (page == pages.LastPage)
            {
                btnNext.Text = "Finish";
                btnCancel.Enabled = false;
            }


            SetContentPage(page);

        }

        private void ExitUpdater()
        {
            Updater.Instance.GetCache().Save();

            if (NeedsRestart)
            {
                Updater.Instance.RestartApp();
            }
            else
            {
                pages.Clear();
                FinishPage page = new FinishPage(this);
                page.UpdateState();
                pages.Add(page);
                SetContentPage(page);
                btnPrevious.Enabled = false;
                btnCancel.Enabled = false;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            bool cancelled = MessageDialog.Show(
                this,
                "Cancel",
                "Cancel updating?",
                "Press Yes to cancel the updating process.\nPress No to keep updating.",
                SystemIcons.Exclamation) == DialogResult.Yes;

            if (cancelled)
                UserCancelled = true;

            if (!cancelled)
                return;

            foreach (IWizardPage page in pages)
            {
                page.Cancel();
                page.UpdateState();
            }

            pages.CurrentPage = pages.LastPage;
            SetContentPage(pages.CurrentPage);

            btnNext.Text = "Finish";
            btnPrevious.Enabled = true;

            OnPageUpdate(pages.CurrentPage);

        }

        private void UpdaterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pages.AllDone())
                return;

            Cancel();

            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
                e.Cancel = true;
        }

        private void linkSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MatthiWare/UpdateLib");
        }
    }
}

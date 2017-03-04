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
            if (this.InvokeRequired)
            {
                Invoke(new _OnPageUpdate(OnPageUpdate), page);
                return;
            }

            if (page.IsDone && !page.IsBusy)
            {
                btnNext.Enabled = true;
                if (page == pages.CurrentPage)
                    btnNext.Focus();
                if (page.NeedsExecution)
                    btnNext.Text = "Next >";
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            IWizardPage currentPage = pages.CurrentPage;
            IWizardPage page = pages.Previous();
            if (page == null)
                return;

            if (!btnNext.Enabled)
                btnNext.Enabled = true;

            if (currentPage.NeedsExecution)
                btnNext.Text = "Next >";

            if (page == pages.FirstPage)
                btnPrevious.Enabled = false;

            SetContentPage(page);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
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

            if (page == pages.LastPage)
                btnNext.Text = "Finish";

            SetContentPage(page);

        }

        private void ExitUpdater()
        {
            if (NeedsRestart)
            {
                //Process current = Process.GetCurrentProcess();
                //Process[] processes = Process.GetProcessesByName(current.ProcessName);
                //foreach(Process p in processes)
                //{
                //    if (current != p)
                //        p.Kill();
                //}

                Application.Restart();
            }
            else
            {
               Updater.Instance.Initialize();

                pages.Clear();
                pages.Add(new FinishPage(this));
                SetContentPage(pages.CurrentPage);
                btnPrevious.Enabled = false;
                this.Close();
            }


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool Cancel()
        {
            MessageDialog dlg = new MessageDialog(
                "Cancel",
                "Cancel updating?",
                "Press Yes to cancel the updating process.\nPress no to keep updating.",
                SystemIcons.Exclamation);

            return dlg.ShowDialog(this) == DialogResult.Yes;
        }

        private void UpdaterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!pages.AllDone())
            {
                bool cancel = Cancel();
                if (!cancel)
                    e.Cancel = true;
            }
        }

        private void linkSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MatthiWare/UpdateLib");
        }
    }
}

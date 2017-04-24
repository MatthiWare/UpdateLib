using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using MatthiWare.UpdateLib.UI;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Generator.Tasks;
using MatthiWare.UpdateLib.Files;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class BuilderPage : PageControlBase
    {
        private FilesPage filesPage;
        private InformationPage infoPage;

        public BuilderPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            saveFileDialog.InitialDirectory = new DirectoryInfo("./Output").FullName; 

            PageControlBase page;
            if (!TestForm.TryGetPage(nameof(FilesPage), out page))
            {
                throw new KeyNotFoundException($"Unable to get {nameof(FilesPage)}");
            }

            filesPage = page as FilesPage;

            if (!TestForm.TryGetPage(nameof(InformationPage), out page))
            {
                throw new KeyNotFoundException($"Unable to get {nameof(InformationPage)}");
            }

            infoPage = page as InformationPage;

            if (!filesPage.IsPageInitialized)
                filesPage.InitializePage(null);

            if (!infoPage.IsPageInitialized)
                infoPage.InitializePage(null);
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(ParentForm) != DialogResult.OK)
                return;

            pbProgress.Value = 0;
            lblProgress.Text = "Progress: 0%";

            Build(saveFileDialog.OpenFile());
        }

        private AsyncTask<UpdateFile> Build(Stream s)
        {
            UpdateGeneratorTask task = new UpdateGeneratorTask(filesPage.Root, infoPage);

            task.TaskProgressChanged += (o, e) =>
            {
                this.InvokeOnUI(p => 
                {
                    lblProgress.Text = $"Progress: {e.ProgressPercentage}%";
                    pbProgress.Value = e.ProgressPercentage;
                });
            };

            task.TaskCompleted += (o, e) =>
            {
                if (e.Cancelled)
                {
                    this.InvokeOnUI(p => lblStatus.Text = $"Status: Cancelled");
                    return;
                }

                if (e.Error != null)
                {
                    this.InvokeOnUI(p => lblStatus.Text = $"Status: Error");

                    new MessageDialog(
                        "Builder",
                        "Build error",
                        "Check the logs for more information",
                        SystemIcons.Error,
                        MessageBoxButtons.OK).ShowDialog(ParentForm);

                    return;
                }

                using (s)
                    task.Result.Save(s);

                this.InvokeOnUI(p => lblStatus.Text = $"Status: Completed");
            };

            lblStatus.Text = "Status: Building..";

            return task.Start();
        }
    }
}

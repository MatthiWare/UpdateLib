using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using UpdateLib.Generator.Tasks;

namespace UpdateLib.Generator
{
    public partial class MainForm : Form
    {

        private DirectoryInfo applicationFolder = new DirectoryInfo("./ApplicationFolder");
        private DirectoryInfo outputFolder = new DirectoryInfo("./Output");

        private ImageList iconList;

        public MainForm()
        {
            InitializeComponent();

            if (!applicationFolder.Exists)
                applicationFolder.Create();

            if (!outputFolder.Exists)
                outputFolder.Create();

            iconList = new ImageList();
            iconList.ImageSize = new Size(24, 24);
            lvItems.SmallImageList = iconList;

            LoadFolder(applicationFolder);
        }

        private LoadDirectoryTask LoadFolder(DirectoryInfo path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!path.Exists) throw new DirectoryNotFoundException($"The directory '{path.FullName}' was not found.");

            LoadDirectoryTask task = new LoadDirectoryTask(lvItems, iconList, path);
            task.Start();

            return task;
        }

        private void Generate()
        {

            UpdateGeneratorTask generator = new UpdateGeneratorTask(applicationFolder);
            
            generator.TaskCompleted += Generator_TaskCompleted;
            generator.TaskProgressChanged += Generator_TaskProgressChanged;

            SetProgressBarValue(0);
            SetProgressBarVisible(true);
            SetWaitCursor(true);

            SetStatusMessage("Generating...");

            generator.Start();
        }

        private void SetWaitCursor(bool val)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetWaitCursor), val);
                return;
            }

            UseWaitCursor = val;
        }

        private void SetProgressBarVisible(bool val)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke(new Action<bool>(SetProgressBarVisible), val);
                return;
            }

            progressBar.Visible = val;
        }

        private void SetProgressBarValue(int val)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke(new Action<int>(SetProgressBarValue), val);
                return;
            }

            progressBar.Value = val;
        }

        private void SetStatusMessage(string msg)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke(new Action<string>(SetStatusMessage), msg);
                return;
            }

            lblStatus.Text = msg;
        }

        private void Generator_TaskProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetStatusMessage($"Generating {e.ProgressPercentage}%");
            SetProgressBarValue(e.ProgressPercentage);
        }

        private void Generator_TaskCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string filePath = string.Concat(outputFolder.FullName, "\\", "updatefile.xml");

            UpdateGeneratorTask gen = (UpdateGeneratorTask)sender;

            gen.Result.Save(filePath);

            SetProgressBarValue(110);

            SetStatusMessage("Build completed");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
           tvProject.ExpandAll();
            tvProject.SelectedNode = tvProject.Nodes["root"].Nodes["nodeInfo"];
        }

        private void tvProject_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "nodeInfo":
                default:

                    break;
                case "nodeFiles":

                    break;
                case "nodeRegistry":

                    break;
            }
        }

        private void buildToolStripButton_Click(object sender, EventArgs e)
        {
            Action generateAction = new Action(Generate);
            
            generateAction.BeginInvoke(new AsyncCallback(r => {
                SetWaitCursor(false);
                //SetProgressBarVisible(false);
                generateAction.EndInvoke(r);
            }), null);
        }

        private void lvItems_DoubleClick(object sender, EventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvItems.SelectedItems[0];

            DirectoryInfo dir = item.Tag as DirectoryInfo;

            if (dir == null)
                return;

            LoadFolder(dir);
        }
    }
}

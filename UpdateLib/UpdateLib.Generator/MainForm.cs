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
            lvItems.SmallImageList = iconList;

            
        }

        private void LoadFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"The directory '{path}' was not found.");


        }

        private void Generate()
        {

            UpdateGenerator generator = new UpdateGenerator(applicationFolder);
            
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

            UpdateGenerator gen = (UpdateGenerator)sender;

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
    }
}

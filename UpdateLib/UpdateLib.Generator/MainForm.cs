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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Generator.Tasks;
using MatthiWare.UpdateLib.UI;

namespace MatthiWare.UpdateLib.Generator
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

            UpdateGeneratorTask generator = new UpdateGeneratorTask(null, null, null);

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
            this.InvokeOnUI(() => UseWaitCursor = val);
        }

        private void SetProgressBarVisible(bool val)
        {
            this.InvokeOnUI(() => progressBar.Visible = val);
        }

        private void SetProgressBarValue(int val)
        {
            this.InvokeOnUI(() => progressBar.Value = val);
        }

        private void SetStatusMessage(string msg)
        {
            this.InvokeOnUI(() => lblStatus.Text = msg);
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

            generateAction.BeginInvoke(new AsyncCallback(r =>
            {
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

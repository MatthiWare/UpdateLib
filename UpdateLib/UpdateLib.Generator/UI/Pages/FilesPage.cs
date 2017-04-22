using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Generator.Data;
using System.IO;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class FilesPage : PageControlBase
    {
        public FilesPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            SuspendLayout();
            
            ilIcons.Images.Add(TreeViewFolderNode.FOLDER_KEY, Properties.Resources.folder_transparent_16px);

            TreeViewFolderNode tvAppFolder = new TreeViewFolderNode("Application Folder");
            TreeViewFolderNode tvAppDataFolder = new TreeViewFolderNode("AppData");

            ListViewItemFolder lvAppFolder = new ListViewItemFolder("Application Folder");
            ListViewItemFolder lvAppDataFolder = new ListViewItemFolder("AppData");

            tvFolders.Nodes.Add(tvAppFolder);
            tvFolders.Nodes.Add(tvAppDataFolder);

            lvFiles.Items.Add(lvAppFolder);
            lvFiles.Items.Add(lvAppDataFolder);

            folderBrowserDialog.Description = "Please select the folder to import";

            ResumeLayout();
        }

       

        private void menuAddFiles_Click(object sender, EventArgs e)
        {
            //TODO: implement
        }

        private void existingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (folderBrowserDialog.ShowDialog(this.ParentForm) != DialogResult.OK)
                return;

            AddExistingFolder(new DirectoryInfo(folderBrowserDialog.SelectedPath));
        }

        private void AddExistingFolder(DirectoryInfo dir)
        {
            TreeViewFolderNode tvFolder = new TreeViewFolderNode(dir.Name);
            ListViewItemFolder lvFolder = new ListViewItemFolder(dir.Name);

            lvFiles.Items.Add(lvFolder);
            tvFolders.Nodes.Add(tvFolder);
        }
    }
}

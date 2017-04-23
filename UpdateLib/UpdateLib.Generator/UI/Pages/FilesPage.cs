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
using MatthiWare.UpdateLib.UI;

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
            if (openFileDialog.ShowDialog(this.ParentForm) != DialogResult.OK)
                return;
            
            AddExistingFileAsync(openFileDialog.FileNames.Select(file => new FileInfo(file)));
        }

        private void existingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this.ParentForm) != DialogResult.OK)
                return;

            AddExistingFolderAsync(new DirectoryInfo(folderBrowserDialog.SelectedPath));
        }

        private AsyncTask AddExistingFolderAsync(DirectoryInfo dir)
        {
            ShowLoader();

            AsyncTask task = AsyncTaskFactory.From(new Action(() =>
           {
               SuspendLayout();

               TreeViewFolderNode tvRoot = new TreeViewFolderNode(dir.Name);
               ListViewItemFolder lvRoot = new ListViewItemFolder(dir.Name);

               foreach (FileInfo file in dir.GetFiles())
                   AddExistingFile(file);

               foreach (DirectoryInfo subDir in dir.GetDirectories())
                   AddExistingFolder(dir, tvRoot, lvRoot);

               this.InvokeOnUI((page) =>
               {
                   lvFiles.Items.Add(lvRoot);
                   tvFolders.Nodes.Add(tvRoot);
               });

               ResumeLayout();
           }), null);

            task.TaskCompleted += (o, e) => { HideLoader(); };

            return task.Start();
        }

        private void AddExistingFolder(DirectoryInfo dir, TreeViewFolderNode tvRoot, ListViewItemFolder lvRoot)
        {

        }

        private AsyncTask AddExistingFileAsync(IEnumerable<FileInfo> files)
        {
            return AsyncTaskFactory.StartNew(new Action(() =>
            {
                foreach (FileInfo file in files)
                    AddExistingFile(file);

            }), null);
        }

        private void AddExistingFile(FileInfo file)
        {
            ListViewItemFile lvFile = new ListViewItemFile(file);

            EnsureExtensionIconExists(file);

            this.InvokeOnUI((page) => lvFiles.Items.Add(lvFile));

        }

        private void EnsureExtensionIconExists(FileInfo file)
        {
            if (ilIcons.Images.ContainsKey(file.Extension))
                return;

            Icon extensionIcon = Icon.ExtractAssociatedIcon(file.FullName);

            this.InvokeOnUI((page) => ilIcons.Images.Add(file.Extension, extensionIcon));
        }
    }
}

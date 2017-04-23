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
using MatthiWare.UpdateLib.Generator.Data.FilesPage;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class FilesPage : PageControlBase
    {
        private GenFolder ProjectRootFolder;
        private GenFolder SelectedFolder;

        public FilesPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            SuspendLayout();

            ilIcons.Images.Add(TreeViewFolderNode.FOLDER_KEY, Properties.Resources.folder_transparent_16px);

            ProjectRootFolder = new GenFolder("Update Project");
            TreeViewFolderNode tvProjectRootFolder = new TreeViewFolderNode("Update Project", ProjectRootFolder);
            tvProjectRootFolder.ContextMenuStrip = contextMenuRightClick;

            tvProjectRootFolder.Folder = ProjectRootFolder;
            ProjectRootFolder.FolderTreeView = tvProjectRootFolder;

            tvFolders.Nodes.Add(tvProjectRootFolder);

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
                this.InvokeOnUI((p) => SuspendLayout());

                AddExistingFolder(dir, ProjectRootFolder, true);

                this.InvokeOnUI((p) =>
                {
                    ProjectRootFolder.FolderTreeView.Expand();
                    ResumeLayout();
                });
            }), null);

            task.TaskCompleted += (o, e) => { HideLoader(); };

            return task.Start();
        }

        private void AddExistingFolder(DirectoryInfo dir, GenFolder parentFolder, bool addToUI = false)
        {
            GenFolder folder = new GenFolder(dir.Name);
            folder.FolderListView = new ListViewItemFolder(dir.Name, folder);
            folder.FolderTreeView = new TreeViewFolderNode(dir.Name, folder);

            folder.FolderTreeView.ContextMenuStrip = contextMenuRightClick;

            if (addToUI)
                this.InvokeOnUI(p => lvFiles.Items.Add(folder.FolderListView));

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                AddExistingFolder(subDir, folder);
            }

            foreach (FileInfo f in dir.GetFiles())
            {
                AddExistingFile(f, folder, addToUI);
            }

            this.InvokeOnUI(p => parentFolder.FolderTreeView.Nodes.Add(folder.FolderTreeView));

            parentFolder.Directories.Add(folder);
        }

        private AsyncTask AddExistingFileAsync(IEnumerable<FileInfo> files)
        {
            return AsyncTaskFactory.StartNew(new Action(() =>
            {
                GenFolder s = SelectedFolder;

                foreach (FileInfo file in files)
                    AddExistingFile(file, s, true);

            }), null);
        }

        private void AddExistingFile(FileInfo f, GenFolder folder, bool addToUI = false)
        {
            GenFile file = new GenFile(f);

            EnsureExtensionIconExists(f);

            folder.Files.Add(file);

            if (addToUI)
                this.InvokeOnUI((page) => lvFiles.Items.Add(file.FileListView));

        }

        private void EnsureExtensionIconExists(FileInfo file)
        {
            if (ilIcons.Images.ContainsKey(file.Extension))
                return;

            Icon extensionIcon = Icon.ExtractAssociatedIcon(file.FullName);

            this.InvokeOnUI((page) => ilIcons.Images.Add(file.Extension, extensionIcon));
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeViewFolderNode node = e.Node as TreeViewFolderNode;

            if (node == null)
                return;

            UpdateSelectedFolder(node?.Folder);
        }

        private void UpdateSelectedFolder(GenFolder folder)
        {
            if (folder == null || folder == SelectedFolder)
                return;

            SelectedFolder = folder;
            Console.WriteLine("Update..");
            lvFiles.SuspendLayout();

            lvFiles.Items.Clear();

            foreach (GenFolder subFolder in folder.Directories)
                lvFiles.Items.Add(subFolder.FolderListView);

            foreach (GenFile subFile in folder.Files)
                lvFiles.Items.Add(subFile.FileListView);

            lvFiles.ResumeLayout();

            folder.FolderTreeView.Expand();
        }

        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            ListViewItemFolder item = lvFiles.SelectedItems[0] as ListViewItemFolder;
            if (item == null)
                return;

            UpdateSelectedFolder(item?.Folder);

        }
    }
}

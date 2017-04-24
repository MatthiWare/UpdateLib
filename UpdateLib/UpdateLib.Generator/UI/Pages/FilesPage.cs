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
        public GenFolder Root { get; set; }

        private GenFile _selectedFile;
        private GenFile SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                if (value != null && !deleteToolStripMenuItem.Enabled)
                    deleteToolStripMenuItem.Enabled = true;
            }
        }
        private GenFolder _selectedFolder;
        private GenFolder SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                _selectedFolder = value;
                if (value != null && !deleteToolStripMenuItem.Enabled)
                    deleteToolStripMenuItem.Enabled = true;
            }
        }

        public FilesPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            SuspendLayout();

            ilIcons.Images.Add(TreeViewFolderNode.FOLDER_KEY, Properties.Resources.folder_transparent_16px);

            Root = new GenFolder("Update Project", null);
            TreeViewFolderNode tvProjectRootFolder = new TreeViewFolderNode("Update Project", Root);
            tvProjectRootFolder.ContextMenuStrip = contextMenuRightClick;

            tvProjectRootFolder.Folder = Root;
            Root.FolderTreeView = tvProjectRootFolder;

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

                AddExistingFolder(dir, Root, true);

                this.InvokeOnUI((p) =>
                {
                    Root.FolderTreeView.Expand();
                    ResumeLayout();
                });
            }), null);

            task.TaskCompleted += (o, e) => { HideLoader(); };

            return task.Start();
        }

        private void AddExistingFolder(DirectoryInfo dir, GenFolder parentFolder, bool addToUI = false)
        {
            GenFolder folder = new GenFolder(dir.Name, parentFolder);
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
            SelectedFile = null;

            if (SelectedFolder == Root && deleteToolStripMenuItem.Enabled)
                deleteToolStripMenuItem.Enabled = false;

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

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputDialog inputDlg = new InputDialog("New folder", "Please enter the folder name: ", MessageBoxButtons.OKCancel);

            if (inputDlg.ShowDialog(ParentForm) != DialogResult.OK && SelectedFolder != null)
                return;

            var name = inputDlg.Input;

            GenFolder folder = new GenFolder(name, SelectedFolder);
            folder.FolderListView = new ListViewItemFolder(name, folder);
            folder.FolderTreeView = new TreeViewFolderNode(name, folder);

            folder.FolderTreeView.ContextMenuStrip = contextMenuRightClick;

            SelectedFolder.Directories.Add(folder);

            this.InvokeOnUI(p =>
            {
                SelectedFolder.FolderTreeView.Nodes.Add(folder.FolderTreeView);
                lvFiles.Items.Add(folder.FolderListView);
            });
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            ListViewItemFile item = lvFiles.SelectedItems[0] as ListViewItemFile;
            if (item == null)
                return;

            SelectedFile = item.File;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuspendLayout();

            deleteToolStripMenuItem.Enabled = false;

            if (SelectedFile != null)
            {
                SelectedFile.ParentFolder.Files.Remove(SelectedFile);
                lvFiles.Items.Remove(SelectedFile.FileListView);
                SelectedFile = null;
            }
            else if (SelectedFolder != null && SelectedFolder != Root)
            {
                SelectedFolder.ParentFolder.Directories.Remove(SelectedFolder);
                tvFolders.Nodes.Remove(SelectedFolder.FolderTreeView);

                foreach (GenFile f in SelectedFolder.Files)
                    lvFiles.Items.Remove(f.FileListView);

                foreach (GenFolder f in SelectedFolder.Directories)
                    lvFiles.Items.Remove(f.FolderListView);

                SelectedFolder = null;
            }
            ResumeLayout();
        }
    }
}

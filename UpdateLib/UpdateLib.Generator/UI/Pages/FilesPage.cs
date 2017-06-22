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
        private const string PROJECT_IMAGE_KEY = "project_key";

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
            ilIcons.Images.Add(PROJECT_IMAGE_KEY, Properties.Resources.project_16px);

            Root = new GenFolder("Update Project", contextMenuRightClick);
            Root.ProtectedFolder = true;
            Root.FolderTreeView.SelectedImageKey = PROJECT_IMAGE_KEY;
            Root.FolderTreeView.ImageKey = PROJECT_IMAGE_KEY;

            GenFolder appFolder = new GenFolder("Application Folder", contextMenuRightClick);
            appFolder.ProtectedFolder = true;
            appFolder.PathVariable = "%appdir%";

            Root.Add(appFolder);

            tvFolders.Nodes.Add(Root.FolderTreeView);

            folderBrowserDialog.Description = "Please select the folder to import";

            UpdateSelectedFolder(Root);

            ResumeLayout();
        }

        private void menuAddFiles_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(ParentForm) != DialogResult.OK)
                return;

            if (SelectedFolder == Root)
                return;

            AddExistingFileAsync(openFileDialog.FileNames.Select(file => new FileInfo(file)));
        }

        private void existingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(ParentForm) != DialogResult.OK)
                return;

            if (SelectedFolder == Root)
                return;

            AddExistingFolderAsync(new DirectoryInfo(folderBrowserDialog.SelectedPath));
        }

        private AsyncTask AddExistingFolderAsync(DirectoryInfo dir)
        {
            ShowLoader();

            AsyncTask task = AsyncTaskFactory.From(new Action(() =>
            {
                this.InvokeOnUI(() => SuspendLayout());

                AddExistingFolder(dir, SelectedFolder, true);

                this.InvokeOnUI(() =>
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
            GenFolder folder = new GenFolder(dir.Name, contextMenuRightClick);

            if (addToUI)
                this.InvokeOnUI(() => lvFiles.Items.Add(folder.FolderListView));

            foreach (DirectoryInfo subDir in dir.GetDirectories())
                AddExistingFolder(subDir, folder);

            foreach (FileInfo f in dir.GetFiles())
                AddExistingFile(f, folder);

            this.InvokeOnUI(() => parentFolder.Add(folder));
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

            folder.Add(file);

            if (addToUI)
                this.InvokeOnUI(() => lvFiles.Items.Add(file.View));

        }

        private void EnsureExtensionIconExists(FileInfo file)
        {
            if (ilIcons.Images.ContainsKey(file.Extension))
                return;

            Icon extensionIcon = Icon.ExtractAssociatedIcon(file.FullName);

            this.InvokeOnUI(() => ilIcons.Images.Add(file.Extension, extensionIcon));
        }

        private void UpdateSelectedFolder(GenFolder folder)
        {
            if (folder == null || folder == SelectedFolder)
                return;

            SelectedFolder = folder;
            SelectedFile = null;

            if (SelectedFolder == Root)
            {
                deleteToolStripMenuItem.Enabled = false;
                menuAddFiles.Enabled = false;
                menuAddFolder.Enabled = false;
            }
            else
            {
                menuAddFiles.Enabled = true;
                menuAddFolder.Enabled = true;
            }

            lvFiles.SuspendLayout();

            lvFiles.Items.Clear();

            foreach (GenFolder subFolder in folder.Directories)
                lvFiles.Items.Add(subFolder.FolderListView);

            foreach (GenFile subFile in folder.Items)
                lvFiles.Items.Add(subFile.View);

            lvFiles.ResumeLayout();

            tvFolders.SelectedNode = folder.FolderTreeView;
            folder.FolderTreeView.Expand();
        }

        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            ListViewFolder item = lvFiles.SelectedItems[0] as ListViewFolder;
            if (item == null)
                return;

            UpdateSelectedFolder(item?.Folder);

        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedFolder == null || SelectedFolder == Root)
                return;

            InputDialog inputDlg = new InputDialog("New folder", "Please enter the folder name: ", MessageBoxButtons.OKCancel);

            if (inputDlg.ShowDialog(ParentForm) != DialogResult.OK && SelectedFolder != null)
                return;

            var name = inputDlg.Input;

            GenFolder folder = new GenFolder(name, contextMenuRightClick);

            this.InvokeOnUI(() =>
            {
                SelectedFolder.Add(folder);
                lvFiles.Items.Add(folder.FolderListView);
            });
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvFiles.SelectedItems[0];

            if (item == null)
                return;

            if (item is ListViewGenItem)
                SelectedFile = (item as ListViewGenItem).Item as GenFile;
            else if (item is ListViewFolder)
                SelectedFolder = (item as ListViewFolder).Folder;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuspendLayout();

            deleteToolStripMenuItem.Enabled = false;

            if (SelectedFile != null)
            {
                SelectedFile.Parent.Remove(SelectedFile);
                SelectedFile = null;
            }
            else if (SelectedFolder != null && SelectedFolder != Root && !SelectedFolder.ProtectedFolder)
            {
                SelectedFolder.ParentFolder.Remove(SelectedFolder);

                lvFiles.Items.Clear();

                GenFolder temp = SelectedFolder;

                UpdateSelectedFolder(SelectedFolder.ParentFolder);

                temp = null;
            }
            ResumeLayout();
        }

        private void tvFolders_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeViewFolderNode node = e.Node as TreeViewFolderNode;

            if (node == null)
                return;

            UpdateSelectedFolder(node?.Folder);
        }
    }
}

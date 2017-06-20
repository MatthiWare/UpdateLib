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
using Microsoft.Win32;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class RegistryPage : PageControlBase
    {
        private const string PROJECT_IMAGE_KEY = "project_key";

        private GenReg m_selectedRegEntry;
        private GenReg SelectedRegEntry
        {
            get { return m_selectedRegEntry; }
            set
            {
                m_selectedRegEntry = value;
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

        public RegistryPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            SuspendLayout();

            ilIcons.Images.Add("REG_BIN", Properties.Resources.reg_bin_16px);
            ilIcons.Images.Add("REG_SZ", Properties.Resources.reg_string_16px);
            ilIcons.Images.Add(TreeViewFolderNode.FOLDER_KEY, Properties.Resources.folder_transparent_16px);

            GenFolder hkey_classes_root = new GenFolder("HKEY_CLASSES_ROOT", contextMenuRightClick);
            GenFolder hkey_current_user = new GenFolder("HKEY_CURRENT_USER", contextMenuRightClick);
            GenFolder hkey_local_machine = new GenFolder("HKEY_LOCAL_MACHINE", contextMenuRightClick);
            GenFolder hkey_users = new GenFolder("HKEY_USERS", contextMenuRightClick);
            GenFolder hkey_current_config = new GenFolder("HKEY_CURRENT_CONFIG", contextMenuRightClick);

            tvFolders.Nodes.Add(hkey_classes_root.FolderTreeView);
            tvFolders.Nodes.Add(hkey_current_user.FolderTreeView);
            tvFolders.Nodes.Add(hkey_local_machine.FolderTreeView);
            tvFolders.Nodes.Add(hkey_users.FolderTreeView);
            tvFolders.Nodes.Add(hkey_current_config.FolderTreeView);

            UpdateSelectedFolder(hkey_classes_root);

            ResumeLayout();
        }

        private AsyncTask AddExistingFolderAsync(DirectoryInfo dir)
        {
            ShowLoader();

            AsyncTask task = AsyncTaskFactory.From(new Action(() =>
            {
                this.InvokeOnUI(() => SuspendLayout());

                AddExistingFolder(dir, SelectedFolder, true);

                this.InvokeOnUI(() => ResumeLayout());

            }), null);

            task.TaskCompleted += (o, e) => HideLoader();

            return task.Start();
        }

        private void AddExistingFolder(DirectoryInfo dir, GenFolder parentFolder, bool addToUI = false)
        {
            GenFolder folder = new GenFolder(dir.Name, contextMenuRightClick);
            parentFolder.Add(folder);

            if (addToUI)
                this.InvokeOnUI(() => lvRegistry.Items.Add(folder.FolderListView));

            foreach (DirectoryInfo subDir in dir.GetDirectories())
                AddExistingFolder(subDir, folder);

            foreach (FileInfo f in dir.GetFiles())
                AddExistingFile(f, folder);

            this.InvokeOnUI(() => parentFolder.FolderTreeView.Nodes.Add(folder.FolderTreeView));
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
            GenReg file = new GenReg("test");

            EnsureExtensionIconExists(f);

            folder.Items.Add(file);

            if (addToUI)
                this.InvokeOnUI(() => lvRegistry.Items.Add(file.View));

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
            SelectedRegEntry = null;

            if (SelectedFolder.IsRoot)
                deleteToolStripMenuItem.Enabled = false;

            lvRegistry.SuspendLayout();

            lvRegistry.Items.Clear();

            foreach (GenFolder subFolder in folder.Directories)
                lvRegistry.Items.Add(subFolder.FolderListView);

            foreach (IGenItem subFile in folder.Items)
                lvRegistry.Items.Add(subFile.View);

            lvRegistry.ResumeLayout();

            folder.FolderTreeView.Expand();
        }

        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lvRegistry.SelectedItems.Count == 0)
                return;

            ListViewFolder item = lvRegistry.SelectedItems[0] as ListViewFolder;
            if (item == null)
                return;

            UpdateSelectedFolder(item?.Folder);

        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvRegistry.SelectedItems.Count == 0)
                return;

            ListViewGenItem item = lvRegistry.SelectedItems[0] as ListViewGenItem;
            if (item == null)
                return;

            SelectedRegEntry = item.Item as GenReg;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuspendLayout();

            deleteToolStripMenuItem.Enabled = false;

            if (SelectedRegEntry != null)
            {
                SelectedRegEntry.Parent.Remove(SelectedRegEntry);
                lvRegistry.Items.Remove(SelectedRegEntry.View);
                SelectedRegEntry = null;
            }
            else if (SelectedFolder != null && !SelectedFolder.IsRoot)
            {
                SelectedFolder.ParentFolder.Remove(SelectedFolder);

                lvRegistry.Items.Clear();

                SelectedFolder = null;
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

        private void menuAddFolder_Click(object sender, EventArgs e)
        {
            if (SelectedFolder == null)
                return;

            InputDialog inputDlg = new InputDialog("New folder", "Please enter the folder name: ", MessageBoxButtons.OKCancel);

            if (inputDlg.ShowDialog(ParentForm) != DialogResult.OK && SelectedFolder != null)
                return;

            var name = inputDlg.Input;

            GenFolder folder = new GenFolder(name, contextMenuRightClick);

            this.InvokeOnUI(() =>
            {
                SelectedFolder.Add(folder);
                SelectedFolder.FolderTreeView.Expand();
                lvRegistry.Items.Add(folder.FolderListView);
            });
        }

        private void stringValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewEntry(RegistryValueKind.String);
        }

        private void binaryValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewEntry(RegistryValueKind.Binary);
        }

        private void dWORDValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewEntry(RegistryValueKind.DWord);
        }

        private void qWORDValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewEntry(RegistryValueKind.QWord);
        }

        private void multiStringValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewEntry(RegistryValueKind.MultiString);
        }

        private void expandableStringValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewEntry(RegistryValueKind.ExpandString);
        }

        private void MakeNewEntry(RegistryValueKind type)
        {
            if (SelectedFolder == null)
                return;

            GenReg item = new GenReg("New Item", type);

            this.InvokeOnUI(() =>
            {
                SelectedFolder.Add(item);
                lvRegistry.Items.Add(item.View);
            });
        }
    }
}

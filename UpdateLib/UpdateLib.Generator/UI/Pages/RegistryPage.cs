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

        public IList<GenFolder> Folders { get; set; }

        public RegistryPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            SuspendLayout();

            Folders = new List<GenFolder>();

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

            Folders.Add(hkey_classes_root);
            Folders.Add(hkey_current_user);
            Folders.Add(hkey_local_machine);
            Folders.Add(hkey_users);
            Folders.Add(hkey_current_config);

            UpdateSelectedFolder(hkey_classes_root);

            ResumeLayout();
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

            tvFolders.SelectedNode = folder.FolderTreeView;
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

            ListViewItem item = lvRegistry.SelectedItems[0];

            if (item == null)
                return;

            if (item is ListViewGenItem)
                SelectedRegEntry = (item as ListViewGenItem).Item as GenReg;
            else if (item is ListViewFolder)
                SelectedFolder = (item as ListViewFolder).Folder;
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

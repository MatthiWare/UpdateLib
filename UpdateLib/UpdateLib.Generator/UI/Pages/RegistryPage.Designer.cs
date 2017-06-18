namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    partial class RegistryPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvRegistry = new System.Windows.Forms.ListView();
            this.clmnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuRightClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Registry";
            // 
            // tvFolders
            // 
            this.tvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.ilIcons;
            this.tvFolders.Location = new System.Drawing.Point(18, 53);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.Size = new System.Drawing.Size(191, 332);
            this.tvFolders.TabIndex = 2;
            this.tvFolders.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvFolders_NodeMouseClick);
            // 
            // ilIcons
            // 
            this.ilIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuRightClick
            // 
            this.contextMenuRightClick.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddFiles,
            this.menuAddFolder,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem});
            this.contextMenuRightClick.Name = "menuTV";
            this.contextMenuRightClick.Size = new System.Drawing.Size(142, 76);
            // 
            // menuAddFiles
            // 
            this.menuAddFiles.Image = global::MatthiWare.UpdateLib.Generator.Properties.Resources.image_transparent_16px;
            this.menuAddFiles.Name = "menuAddFiles";
            this.menuAddFiles.Size = new System.Drawing.Size(141, 22);
            this.menuAddFiles.Text = "Add File(s)";
            this.menuAddFiles.Click += new System.EventHandler(this.menuAddFiles_Click);
            // 
            // menuAddFolder
            // 
            this.menuAddFolder.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.existingFolderToolStripMenuItem});
            this.menuAddFolder.Image = global::MatthiWare.UpdateLib.Generator.Properties.Resources.folder_transparent_16px;
            this.menuAddFolder.Name = "menuAddFolder";
            this.menuAddFolder.Size = new System.Drawing.Size(141, 22);
            this.menuAddFolder.Text = "Add Folder";
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // existingFolderToolStripMenuItem
            // 
            this.existingFolderToolStripMenuItem.Name = "existingFolderToolStripMenuItem";
            this.existingFolderToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.existingFolderToolStripMenuItem.Text = "Existing Folder";
            this.existingFolderToolStripMenuItem.Click += new System.EventHandler(this.existingFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(138, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Image = global::MatthiWare.UpdateLib.Generator.Properties.Resources.cross;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // lvRegistry
            // 
            this.lvRegistry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRegistry.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnName,
            this.clmnType,
            this.clmnValue});
            this.lvRegistry.ContextMenuStrip = this.contextMenuRightClick;
            this.lvRegistry.FullRowSelect = true;
            this.lvRegistry.Location = new System.Drawing.Point(215, 53);
            this.lvRegistry.MultiSelect = false;
            this.lvRegistry.Name = "lvRegistry";
            this.lvRegistry.Size = new System.Drawing.Size(577, 332);
            this.lvRegistry.SmallImageList = this.ilIcons;
            this.lvRegistry.TabIndex = 3;
            this.lvRegistry.UseCompatibleStateImageBehavior = false;
            this.lvRegistry.View = System.Windows.Forms.View.Details;
            this.lvRegistry.SelectedIndexChanged += new System.EventHandler(this.lvFiles_SelectedIndexChanged);
            this.lvRegistry.DoubleClick += new System.EventHandler(this.lvFiles_DoubleClick);
            // 
            // clmnName
            // 
            this.clmnName.Text = "Name";
            this.clmnName.Width = 150;
            // 
            // clmnType
            // 
            this.clmnType.Text = "Type";
            this.clmnType.Width = 93;
            // 
            // clmnValue
            // 
            this.clmnValue.Text = "Value";
            this.clmnValue.Width = 250;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.ReadOnlyChecked = true;
            this.openFileDialog.Title = "Add files to be included in the updater";
            // 
            // RegistryPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvRegistry);
            this.Controls.Add(this.tvFolders);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(589, 233);
            this.Name = "RegistryPage";
            this.Size = new System.Drawing.Size(810, 403);
            this.contextMenuRightClick.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.ListView lvRegistry;
        private System.Windows.Forms.ColumnHeader clmnName;
        private System.Windows.Forms.ColumnHeader clmnType;
        private System.Windows.Forms.ColumnHeader clmnValue;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ContextMenuStrip contextMenuRightClick;
        private System.Windows.Forms.ToolStripMenuItem menuAddFiles;
        private System.Windows.Forms.ToolStripMenuItem menuAddFolder;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem existingFolderToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}

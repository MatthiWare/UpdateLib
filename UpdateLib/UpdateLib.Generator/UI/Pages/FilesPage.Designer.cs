﻿namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    partial class FilesPage
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
            this.lvFiles = new System.Windows.Forms.ListView();
            this.clmnIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuRightClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = " Files and folders";
            // 
            // tvFolders
            // 
            this.tvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvFolders.ContextMenuStrip = this.contextMenuRightClick;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.ilIcons;
            this.tvFolders.Location = new System.Drawing.Point(18, 53);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.Size = new System.Drawing.Size(191, 331);
            this.tvFolders.TabIndex = 2;
            // 
            // ilIcons
            // 
            this.ilIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvFiles
            // 
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnIcon,
            this.clmnName,
            this.clmnDate,
            this.clmnType,
            this.clmnSize});
            this.lvFiles.ContextMenuStrip = this.contextMenuRightClick;
            this.lvFiles.Location = new System.Drawing.Point(215, 53);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(514, 331);
            this.lvFiles.SmallImageList = this.ilIcons;
            this.lvFiles.TabIndex = 3;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // clmnIcon
            // 
            this.clmnIcon.Text = "";
            // 
            // clmnName
            // 
            this.clmnName.Text = "Name";
            // 
            // clmnDate
            // 
            this.clmnDate.Text = "Last Modified";
            // 
            // clmnType
            // 
            this.clmnType.Text = "Type";
            // 
            // clmnSize
            // 
            this.clmnSize.Text = "Size";
            // 
            // contextMenuRightClick
            // 
            this.contextMenuRightClick.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddFiles,
            this.menuAddFolder});
            this.contextMenuRightClick.Name = "menuTV";
            this.contextMenuRightClick.Size = new System.Drawing.Size(142, 48);
            // 
            // menuAddFiles
            // 
            this.menuAddFiles.Image = global::MatthiWare.UpdateLib.Generator.Properties.Resources.image_transparent_16px;
            this.menuAddFiles.Name = "menuAddFiles";
            this.menuAddFiles.Size = new System.Drawing.Size(141, 22);
            this.menuAddFiles.Text = "Add File(s)";
            // 
            // menuAddFolder
            // 
            this.menuAddFolder.Image = global::MatthiWare.UpdateLib.Generator.Properties.Resources.folder_transparent_16px;
            this.menuAddFolder.Name = "menuAddFolder";
            this.menuAddFolder.Size = new System.Drawing.Size(141, 22);
            this.menuAddFolder.Text = "Add Folder";
            // 
            // FilesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.tvFolders);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FilesPage";
            this.Size = new System.Drawing.Size(747, 402);
            this.contextMenuRightClick.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader clmnIcon;
        private System.Windows.Forms.ColumnHeader clmnName;
        private System.Windows.Forms.ColumnHeader clmnDate;
        private System.Windows.Forms.ColumnHeader clmnType;
        private System.Windows.Forms.ColumnHeader clmnSize;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ContextMenuStrip contextMenuRightClick;
        private System.Windows.Forms.ToolStripMenuItem menuAddFiles;
        private System.Windows.Forms.ToolStripMenuItem menuAddFolder;
    }
}

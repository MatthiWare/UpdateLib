namespace MatthiWare.UpdateLib.UI.Components
{
    partial class RollbackPage
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Installed version", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Local versions", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Available versions", System.Windows.Forms.HorizontalAlignment.Left);
            this.lblHeader = new System.Windows.Forms.Label();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.lvItems = new System.Windows.Forms.ListView();
            this.clmnVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(129)))), ((int)(((byte)(181)))));
            this.lblHeader.Location = new System.Drawing.Point(17, 18);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(359, 25);
            this.lblHeader.TabIndex = 3;
            this.lblHeader.Text = "Rollback/repair manager %AppName%";
            // 
            // ilIcons
            // 
            this.ilIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvItems
            // 
            this.lvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnVersion,
            this.clmnStatus,
            this.clmnProgress,
            this.clmnDescription,
            this.clmnPath});
            this.lvItems.FullRowSelect = true;
            listViewGroup1.Header = "Installed version";
            listViewGroup1.Name = "lvgInstalled";
            listViewGroup2.Header = "Local versions";
            listViewGroup2.Name = "lvgLocal";
            listViewGroup3.Header = "Available versions";
            listViewGroup3.Name = "lvgAvailable";
            this.lvItems.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.lvItems.Location = new System.Drawing.Point(17, 46);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(505, 282);
            this.lvItems.TabIndex = 4;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            // 
            // clmnVersion
            // 
            this.clmnVersion.Text = "Version";
            this.clmnVersion.Width = 125;
            // 
            // clmnStatus
            // 
            this.clmnStatus.Text = "Status";
            this.clmnStatus.Width = 131;
            // 
            // clmnProgress
            // 
            this.clmnProgress.Text = "Progress";
            this.clmnProgress.Width = 85;
            // 
            // clmnDescription
            // 
            this.clmnDescription.Text = "Description";
            this.clmnDescription.Width = 100;
            // 
            // clmnPath
            // 
            this.clmnPath.Text = "Path";
            this.clmnPath.Width = 350;
            // 
            // RollbackPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lvItems);
            this.Controls.Add(this.lblHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RollbackPage";
            this.Size = new System.Drawing.Size(538, 341);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.ColumnHeader clmnVersion;
        private System.Windows.Forms.ColumnHeader clmnStatus;
        private System.Windows.Forms.ColumnHeader clmnProgress;
        private System.Windows.Forms.ColumnHeader clmnDescription;
        private System.Windows.Forms.ColumnHeader clmnPath;
    }
}

﻿namespace MatthiWare.UpdateLib.UI.Components
{
    partial class UpdatePage
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblSubheader = new System.Windows.Forms.Label();
            this.lvItems = new System.Windows.Forms.ListView();
            this.clmnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(129)))), ((int)(((byte)(181)))));
            this.lblHeader.Location = new System.Drawing.Point(14, 14);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(140, 25);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Apply updates";
            // 
            // lblSubheader
            // 
            this.lblSubheader.AutoSize = true;
            this.lblSubheader.Location = new System.Drawing.Point(14, 52);
            this.lblSubheader.Name = "lblSubheader";
            this.lblSubheader.Size = new System.Drawing.Size(252, 17);
            this.lblSubheader.TabIndex = 1;
            this.lblSubheader.Text = "Press Update to start the update process.";
            // 
            // lvItems
            // 
            this.lvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnName,
            this.clmnStatus,
            this.clmnProgress,
            this.clmnDescription,
            this.clmnPath});
            this.lvItems.FullRowSelect = true;
            this.lvItems.Location = new System.Drawing.Point(17, 72);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(505, 255);
            this.lvItems.TabIndex = 2;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            // 
            // clmnName
            // 
            this.clmnName.Text = "File name";
            this.clmnName.Width = 125;
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
            // ilIcons
            // 
            this.ilIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // UpdatePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lvItems);
            this.Controls.Add(this.lblSubheader);
            this.Controls.Add(this.lblHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UpdatePage";
            this.Size = new System.Drawing.Size(538, 341);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblSubheader;
        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ColumnHeader clmnName;
        private System.Windows.Forms.ColumnHeader clmnStatus;
        private System.Windows.Forms.ColumnHeader clmnProgress;
        private System.Windows.Forms.ColumnHeader clmnPath;
        private System.Windows.Forms.ColumnHeader clmnDescription;
    }
}

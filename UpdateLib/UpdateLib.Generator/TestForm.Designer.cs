﻿namespace MatthiWare.UpdateLib.Generator
{
    partial class TestForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.SidebarPanel = new System.Windows.Forms.Panel();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.btnTabBuild = new MatthiWare.UpdateLib.Generator.UI.FlatButton();
            this.btnTabRegistry = new MatthiWare.UpdateLib.Generator.UI.FlatButton();
            this.btnTabFiles = new MatthiWare.UpdateLib.Generator.UI.FlatButton();
            this.btnTabInformation = new MatthiWare.UpdateLib.Generator.UI.FlatButton();
            this.HeaderPanel = new MatthiWare.UpdateLib.Generator.UI.MoveablePanel();
            this.pbMinimize = new MatthiWare.UpdateLib.Generator.UI.HoverPictureBox();
            this.pbMaximize = new MatthiWare.UpdateLib.Generator.UI.HoverPictureBox();
            this.pbClose = new MatthiWare.UpdateLib.Generator.UI.HoverPictureBox();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.elipseComponent1 = new MatthiWare.UpdateLib.Generator.UI.ElipseComponent(this.components);
            this.SidebarPanel.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMaximize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // SidebarPanel
            // 
            this.SidebarPanel.BackColor = System.Drawing.Color.DarkGray;
            this.SidebarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SidebarPanel.Controls.Add(this.btnTabBuild);
            this.SidebarPanel.Controls.Add(this.btnTabRegistry);
            this.SidebarPanel.Controls.Add(this.btnTabFiles);
            this.SidebarPanel.Controls.Add(this.btnTabInformation);
            this.SidebarPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SidebarPanel.Location = new System.Drawing.Point(0, 33);
            this.SidebarPanel.Name = "SidebarPanel";
            this.SidebarPanel.Size = new System.Drawing.Size(233, 505);
            this.SidebarPanel.TabIndex = 1;
            // 
            // ContentPanel
            // 
            this.ContentPanel.AutoScroll = true;
            this.ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentPanel.Location = new System.Drawing.Point(233, 33);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Size = new System.Drawing.Size(806, 505);
            this.ContentPanel.TabIndex = 2;
            // 
            // btnTabBuild
            // 
            this.btnTabBuild.ActiveItem = false;
            this.btnTabBuild.BackHoverColor = System.Drawing.Color.LightGray;
            this.btnTabBuild.BackSelectedColor = System.Drawing.Color.DimGray;
            this.btnTabBuild.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTabBuild.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabBuild.InfoImage = ((System.Drawing.Image)(resources.GetObject("btnTabBuild.InfoImage")));
            this.btnTabBuild.Location = new System.Drawing.Point(0, 189);
            this.btnTabBuild.Name = "btnTabBuild";
            this.btnTabBuild.Size = new System.Drawing.Size(231, 63);
            this.btnTabBuild.TabIndex = 3;
            this.btnTabBuild.Text = "Build";
            this.btnTabBuild.Click += new System.EventHandler(this.btnTabBuild_Click);
            // 
            // btnTabRegistry
            // 
            this.btnTabRegistry.ActiveItem = false;
            this.btnTabRegistry.BackHoverColor = System.Drawing.Color.LightGray;
            this.btnTabRegistry.BackSelectedColor = System.Drawing.Color.DimGray;
            this.btnTabRegistry.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTabRegistry.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabRegistry.InfoImage = global::MatthiWare.UpdateLib.Generator.Properties.Resources.Registry_Editor_32px;
            this.btnTabRegistry.Location = new System.Drawing.Point(0, 126);
            this.btnTabRegistry.Name = "btnTabRegistry";
            this.btnTabRegistry.Size = new System.Drawing.Size(231, 63);
            this.btnTabRegistry.TabIndex = 2;
            this.btnTabRegistry.Text = "Registry";
            this.btnTabRegistry.Click += new System.EventHandler(this.btnTabRegistry_Click);
            // 
            // btnTabFiles
            // 
            this.btnTabFiles.ActiveItem = false;
            this.btnTabFiles.BackHoverColor = System.Drawing.Color.LightGray;
            this.btnTabFiles.BackSelectedColor = System.Drawing.Color.DimGray;
            this.btnTabFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTabFiles.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabFiles.InfoImage = ((System.Drawing.Image)(resources.GetObject("btnTabFiles.InfoImage")));
            this.btnTabFiles.Location = new System.Drawing.Point(0, 63);
            this.btnTabFiles.Name = "btnTabFiles";
            this.btnTabFiles.Size = new System.Drawing.Size(231, 63);
            this.btnTabFiles.TabIndex = 1;
            this.btnTabFiles.Text = "Files";
            this.btnTabFiles.Click += new System.EventHandler(this.flatButton2_Click);
            // 
            // btnTabInformation
            // 
            this.btnTabInformation.ActiveItem = false;
            this.btnTabInformation.BackHoverColor = System.Drawing.Color.LightGray;
            this.btnTabInformation.BackSelectedColor = System.Drawing.Color.DimGray;
            this.btnTabInformation.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTabInformation.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabInformation.InfoImage = ((System.Drawing.Image)(resources.GetObject("btnTabInformation.InfoImage")));
            this.btnTabInformation.Location = new System.Drawing.Point(0, 0);
            this.btnTabInformation.Name = "btnTabInformation";
            this.btnTabInformation.Size = new System.Drawing.Size(231, 63);
            this.btnTabInformation.TabIndex = 0;
            this.btnTabInformation.Text = "Update Information";
            this.btnTabInformation.Click += new System.EventHandler(this.flatButton1_Click);
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(99)))), ((int)(((byte)(133)))));
            this.HeaderPanel.Controls.Add(this.pbMinimize);
            this.HeaderPanel.Controls.Add(this.pbMaximize);
            this.HeaderPanel.Controls.Add(this.pbClose);
            this.HeaderPanel.Controls.Add(this.pbIcon);
            this.HeaderPanel.Controls.Add(this.lblTitle);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.ParentForm = this;
            this.HeaderPanel.Size = new System.Drawing.Size(1039, 33);
            this.HeaderPanel.TabIndex = 1;
            // 
            // pbMinimize
            // 
            this.pbMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMinimize.Image = ((System.Drawing.Image)(resources.GetObject("pbMinimize.Image")));
            this.pbMinimize.Location = new System.Drawing.Point(965, 5);
            this.pbMinimize.Name = "pbMinimize";
            this.pbMinimize.Size = new System.Drawing.Size(24, 24);
            this.pbMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMinimize.TabIndex = 4;
            this.pbMinimize.TabStop = false;
            this.pbMinimize.Click += new System.EventHandler(this.pbMinimize_Click);
            // 
            // pbMaximize
            // 
            this.pbMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMaximize.BackColor = System.Drawing.Color.Transparent;
            this.pbMaximize.Image = ((System.Drawing.Image)(resources.GetObject("pbMaximize.Image")));
            this.pbMaximize.Location = new System.Drawing.Point(988, 5);
            this.pbMaximize.Name = "pbMaximize";
            this.pbMaximize.Size = new System.Drawing.Size(24, 24);
            this.pbMaximize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMaximize.TabIndex = 3;
            this.pbMaximize.TabStop = false;
            this.pbMaximize.Click += new System.EventHandler(this.pbMaximize_Click);
            // 
            // pbClose
            // 
            this.pbClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbClose.Image = ((System.Drawing.Image)(resources.GetObject("pbClose.Image")));
            this.pbClose.Location = new System.Drawing.Point(1011, 5);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(24, 24);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbClose.TabIndex = 2;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            // 
            // pbIcon
            // 
            this.pbIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbIcon.BackgroundImage")));
            this.pbIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbIcon.Location = new System.Drawing.Point(5, 5);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(24, 24);
            this.pbIcon.TabIndex = 1;
            this.pbIcon.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTitle.Location = new System.Drawing.Point(31, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(157, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Update Generator";
            // 
            // elipseComponent1
            // 
            this.elipseComponent1.Control = this;
            this.elipseComponent1.Radius = 5;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1039, 538);
            this.Controls.Add(this.ContentPanel);
            this.Controls.Add(this.SidebarPanel);
            this.Controls.Add(this.HeaderPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestForm";
            this.Click += new System.EventHandler(this.TestForm_Click);
            this.SidebarPanel.ResumeLayout(false);
            this.HeaderPanel.ResumeLayout(false);
            this.HeaderPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMaximize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UI.ElipseComponent elipseComponent1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel SidebarPanel;
        private UI.MoveablePanel HeaderPanel;
        private System.Windows.Forms.PictureBox pbIcon;
        private UI.HoverPictureBox pbClose;
        private UI.HoverPictureBox pbMinimize;
        private UI.HoverPictureBox pbMaximize;
        private UI.FlatButton btnTabInformation;
        private UI.FlatButton btnTabFiles;
        private UI.FlatButton btnTabBuild;
        internal System.Windows.Forms.Panel ContentPanel;
        private UI.FlatButton btnTabRegistry;
    }
}
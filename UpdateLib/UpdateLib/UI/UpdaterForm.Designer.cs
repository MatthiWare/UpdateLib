namespace MatthiWare.UpdateLib.UI
{
    partial class UpdaterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdaterForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUpdateCancel = new System.Windows.Forms.Button();
            this.lvItems = new System.Windows.Forms.ListView();
            this.clmnImg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnExtra = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.btnUpdateCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 299);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(481, 51);
            this.panel1.TabIndex = 0;
            // 
            // btnUpdateCancel
            // 
            this.btnUpdateCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateCancel.Location = new System.Drawing.Point(394, 14);
            this.btnUpdateCancel.Name = "btnUpdateCancel";
            this.btnUpdateCancel.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateCancel.TabIndex = 0;
            this.btnUpdateCancel.Text = "Update";
            this.btnUpdateCancel.UseVisualStyleBackColor = true;
            this.btnUpdateCancel.Click += new System.EventHandler(this.btnUpdateCancel_Click);
            // 
            // lvItems
            // 
            this.lvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnImg,
            this.clmnName,
            this.clmnStatus,
            this.clmnProgress,
            this.clmnExtra});
            this.lvItems.Location = new System.Drawing.Point(12, 29);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(457, 257);
            this.lvItems.SmallImageList = this.imgList;
            this.lvItems.TabIndex = 1;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            // 
            // clmnImg
            // 
            this.clmnImg.Text = "";
            this.clmnImg.Width = 22;
            // 
            // clmnName
            // 
            this.clmnName.Text = "File name";
            this.clmnName.Width = 113;
            // 
            // clmnStatus
            // 
            this.clmnStatus.Text = "Status";
            this.clmnStatus.Width = 84;
            // 
            // clmnProgress
            // 
            this.clmnProgress.Text = "Progress";
            // 
            // clmnExtra
            // 
            this.clmnExtra.Text = "Extra options";
            this.clmnExtra.Width = 84;
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "status_done");
            this.imgList.Images.SetKeyName(1, "status_download");
            this.imgList.Images.SetKeyName(2, "status_error");
            this.imgList.Images.SetKeyName(3, "status_info");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Files to update";
            // 
            // UpdaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 350);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvItems);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdaterForm";
            this.Text = "Updater";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnUpdateCancel;
        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.ColumnHeader clmnImg;
        private System.Windows.Forms.ColumnHeader clmnName;
        private System.Windows.Forms.ColumnHeader clmnStatus;
        private System.Windows.Forms.ColumnHeader clmnProgress;
        private System.Windows.Forms.ColumnHeader clmnExtra;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imgList;
    }
}
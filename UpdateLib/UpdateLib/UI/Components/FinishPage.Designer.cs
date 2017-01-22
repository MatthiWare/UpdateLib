namespace MatthiWare.UpdateLib.UI.Components
{
    partial class FinishPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinishPage));
            this.txtFinished = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.Label();
            this.cbRestart = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtFinished
            // 
            this.txtFinished.AutoSize = true;
            this.txtFinished.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFinished.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.txtFinished.Location = new System.Drawing.Point(12, 12);
            this.txtFinished.Name = "txtFinished";
            this.txtFinished.Size = new System.Drawing.Size(85, 25);
            this.txtFinished.TabIndex = 0;
            this.txtFinished.Text = "Finished";
            // 
            // txtDescription
            // 
            this.txtDescription.AutoSize = true;
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.Location = new System.Drawing.Point(14, 52);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(433, 136);
            this.txtDescription.TabIndex = 1;
            this.txtDescription.Text = resources.GetString("txtDescription.Text");
            // 
            // cbRestart
            // 
            this.cbRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbRestart.AutoSize = true;
            this.cbRestart.Checked = true;
            this.cbRestart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRestart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRestart.Location = new System.Drawing.Point(17, 202);
            this.cbRestart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRestart.Name = "cbRestart";
            this.cbRestart.Size = new System.Drawing.Size(136, 21);
            this.cbRestart.TabIndex = 2;
            this.cbRestart.Text = "Restart application";
            this.cbRestart.UseVisualStyleBackColor = true;
            // 
            // FinishPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbRestart);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtFinished);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FinishPage";
            this.Size = new System.Drawing.Size(701, 245);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtFinished;
        private System.Windows.Forms.Label txtDescription;
        private System.Windows.Forms.CheckBox cbRestart;
    }
}

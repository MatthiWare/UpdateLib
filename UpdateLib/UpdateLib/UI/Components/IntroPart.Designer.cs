namespace MatthiWare.UpdateLib.UI.Components
{
    partial class IntroPart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntroPart));
            this.txtWelcome = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtWelcome
            // 
            this.txtWelcome.AutoSize = true;
            this.txtWelcome.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWelcome.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.txtWelcome.Location = new System.Drawing.Point(12, 13);
            this.txtWelcome.Name = "txtWelcome";
            this.txtWelcome.Size = new System.Drawing.Size(360, 25);
            this.txtWelcome.TabIndex = 0;
            this.txtWelcome.Text = "Welcome to the %AppName% Updater!";
            // 
            // txtDesc
            // 
            this.txtDesc.AutoSize = true;
            this.txtDesc.Location = new System.Drawing.Point(14, 52);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(425, 119);
            this.txtDesc.TabIndex = 1;
            this.txtDesc.Text = resources.GetString("txtDesc.Text");
            // 
            // IntroPart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.txtWelcome);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "IntroPart";
            this.Size = new System.Drawing.Size(515, 232);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtWelcome;
        private System.Windows.Forms.Label txtDesc;
    }
}

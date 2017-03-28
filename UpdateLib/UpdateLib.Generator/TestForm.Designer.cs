namespace UpdateLib.Generator
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
            this.gradientPanel1 = new UpdateLib.Generator.UI.GradientPanel();
            this.elipseComponent1 = new UpdateLib.Generator.UI.ElipseComponent(this.components);
            this.SuspendLayout();
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("gradientPanel1.BackgroundImage")));
            this.gradientPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.gradientPanel1.GradientBottomLeft = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(67)))), ((int)(((byte)(133)))));
            this.gradientPanel1.GradientBottomRight = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(99)))), ((int)(((byte)(149)))));
            this.gradientPanel1.GradientTopLeft = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(99)))), ((int)(((byte)(149)))));
            this.gradientPanel1.GradientTopRight = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(67)))), ((int)(((byte)(133)))));
            this.gradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Quality = 200;
            this.gradientPanel1.Size = new System.Drawing.Size(200, 350);
            this.gradientPanel1.TabIndex = 0;
            // 
            // elipseComponent1
            // 
            this.elipseComponent1.Control = this;
            this.elipseComponent1.Radius = 5;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 350);
            this.Controls.Add(this.gradientPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestForm";
            this.Click += new System.EventHandler(this.TestForm_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.ElipseComponent elipseComponent1;
        private UI.GradientPanel gradientPanel1;
    }
}
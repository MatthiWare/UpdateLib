﻿namespace TestApp
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.updaterControl2 = new MatthiWare.UpdateLib.Controls.UpdaterControl();
            this.updaterControl1 = new MatthiWare.UpdateLib.Controls.UpdaterControl();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(206, 119);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // updaterControl2
            // 
            this.updaterControl2.BackColor = System.Drawing.Color.Transparent;
            this.updaterControl2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updaterControl2.Location = new System.Drawing.Point(454, 12);
            this.updaterControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.updaterControl2.Name = "updaterControl2";
            this.updaterControl2.Size = new System.Drawing.Size(20, 20);
            this.updaterControl2.TabIndex = 2;
            // 
            // updaterControl1
            // 
            this.updaterControl1.BackColor = System.Drawing.Color.Transparent;
            this.updaterControl1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updaterControl1.Location = new System.Drawing.Point(12, 228);
            this.updaterControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.updaterControl1.Name = "updaterControl1";
            this.updaterControl1.Size = new System.Drawing.Size(20, 20);
            this.updaterControl1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 261);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.updaterControl2);
            this.Controls.Add(this.updaterControl1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Test app";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private MatthiWare.UpdateLib.Controls.UpdaterControl updaterControl1;
        private MatthiWare.UpdateLib.Controls.UpdaterControl updaterControl2;
        private System.Windows.Forms.Button button2;
    }
}


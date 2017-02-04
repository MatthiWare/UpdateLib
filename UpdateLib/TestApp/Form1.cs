using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            HashCacheFile file = new HashCacheFile();
            file.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = new MessageDialog(
                "Test title",
                "Version 1.0.0.0 available",
                "Download update now?\nPress yes to download or no to cancel.", SystemIcons.Question).ShowDialog(this);

            if (result == DialogResult.OK)
            {
                UpdateFile updateFile = new UpdateFile("1.0.0.0");
                updateFile.Files.Add(new FileEntry("test"));
                updateFile.Files.Add(new FileEntry("Updater.exe"));
                updateFile.Files.Add(new FileEntry("App.exe"));

                UpdaterForm updateForm = UpdaterForm.GetCached(updateFile);
                updateForm.ShowDialog(this);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateFile updateFile = new UpdateFile("1.0.0.0");
            updateFile.Files.Add(new FileEntry("test"));
            updateFile.Files.Add(new FileEntry("Updater.exe"));
            updateFile.Files.Add(new FileEntry("App.exe"));

            UpdaterForm updateForm = UpdaterForm.GetCached(updateFile);
            updateForm.ShowDialog(this);
        }
    }
}

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

            HashCacheEntry entry = new HashCacheEntry("UpdateLib.dll");
            HashCacheEntry entry2 = new HashCacheEntry("TestApp.vshost.exe");
            HashCacheEntry entry3 = new HashCacheEntry("TestApp.pdb");
            HashCacheEntry entry4 = new HashCacheEntry("TestApp.exe");

            file.Items.Add(entry);
            file.Items.Add(entry2);
            file.Items.Add(entry3);
            file.Items.Add(entry4);
            file.Save();

            HashCacheFile returnFile = HashCacheFile.Load();
            Console.WriteLine(">>> HashCacheFile <<<");
            foreach (HashCacheEntry e in returnFile.Items)
            {
                Console.WriteLine("--- HashCacheEntry ---");
                Console.WriteLine("Hash: {0}", e.Hash);
                Console.WriteLine("FilePath: {0}", e.FilePath);
                Console.WriteLine("Time: {0}", DateTime.FromBinary(e.Ticks));
                Console.WriteLine("---------------------");
            }
            Console.WriteLine(">>>>>>>> EOF <<<<<<<<");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = new MessageDialog(
                "Test title",
                "Version 1.0.0.0 available",
                "Download update now?\nPress yes to download or no to cancel.", SystemIcons.Question).ShowDialog(this);

            if (result == DialogResult.OK)
            {
                UpdateFile updateFile = new UpdateFile();
                updateFile.VersionString = "1.0.0.0";
                updateFile.ApplicationDirectory.Files.Add(new FileEntry("test"));
                updateFile.ApplicationDirectory.Files.Add(new FileEntry("Updater.exe"));
                updateFile.ApplicationDirectory.Files.Add(new FileEntry("App.exe"));

                UpdaterForm updateForm = new UpdaterForm(updateFile);
                updateForm.ShowDialog(this);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateFile updateFile = new UpdateFile();
            updateFile.VersionString = "1.0.0.0";
            updateFile.ApplicationDirectory.Files.Add(new FileEntry("test"));
            updateFile.ApplicationDirectory.Files.Add(new FileEntry("Updater.exe"));
            updateFile.ApplicationDirectory.Files.Add(new FileEntry("App.exe"));

            UpdaterForm updateForm = new UpdaterForm(updateFile);
            updateForm.ShowDialog(this);
        }
    }
}

using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private Updater updater;

        public Form1()
        {
            InitializeComponent();

            updater = Updater.Instance;
            updater.UpdateURL= "https://dl.dropboxusercontent.com/u/30635736/UpdateLib/Dev/updatefile.xml";

            updater.Initialize();
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

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateFile file = UpdateFile.Load("../../../UpdateLib.Generator/bin/Debug/Output/updatefile.xml");
            UpdaterForm updaterForm = new UpdaterForm(file);
            updaterForm.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            updater.CheckForUpdates();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Testfile1: {0}", File.ReadAllText("./data/testfile1.txt"));
            Console.WriteLine("Testfile2: {0}", File.ReadAllText("./data/testfile2.txt"));
            Console.WriteLine("Testfile3: {0}", File.ReadAllText("./data/testfile3.txt"));
        }
    }
}

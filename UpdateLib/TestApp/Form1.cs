using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TestApp.Testing;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Updater.Instance.CheckForUpdatesCompleted += Instance_CheckForUpdatesCompleted;
        }

        private void Instance_CheckForUpdatesCompleted(object sender, CheckForUpdatesCompletedEventArgs e)
        {
            checkForUpdatesToolStripMenuItem.Enabled = true;

            //if (e.Cancelled || e.Error != null)
            //{
            //    this.InvokeOnUI(() => MessageDialog.Show(
            //        this,
            //        "Updater",
            //        e.Cancelled ? "Cancelled" : "Error",
            //        e.Cancelled ? "Update got cancelled" : "Please check the logs for more information.",
            //        e.Cancelled ? SystemIcons.Warning : SystemIcons.Error,
            //        MessageBoxButtons.OK));

            //    return;
            //}

            //if (!e.UpdateAvailable)
            //{
            //    this.InvokeOnUI(() =>
            //    MessageDialog.Show(
            //        this,
            //        "Updater",
            //        "No update available!",
            //        $"You already have the latest version ({e.LatestVersion}).",
            //        SystemIcons.Information,
            //        MessageBoxButtons.OK));

            //    return;
            //}
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdatesToolStripMenuItem.Enabled = false;

            AsyncTask task = Updater.Instance.CheckForUpdatesAsync();
            //task.Cancel();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = ReadFile("data/testfile1.txt");
            label2.Text = ReadFile("data/testfile2.txt");
            label3.Text = ReadFileAndKeepStreamOpen("data/testfile3.txt");
        }

        private string ReadFile(string file)
        {
            if (!File.Exists(file))
                return "ERROR: File doesn't exist..";

            string[] lines = File.ReadAllLines(file);

            return string.Join(", ", lines);
        }

        FileStream fs;
        /// <summary>
        /// Bad code that keeps the file open & locked
        /// Purpose: to demonstrate the updater still works on locked files.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string ReadFileAndKeepStreamOpen(string file)
        {
            if (!File.Exists(file))
                return "ERROR: File doesn't exist..";

            fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            StreamReader sr = new StreamReader(fs);
            string text = sr.ReadToEnd();

            return text;
        }
    }
}

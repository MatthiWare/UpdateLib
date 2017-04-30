using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
            this.InvokeOnUI(() => checkForUpdatesToolStripMenuItem.Enabled = true);

            if (e.Cancelled || e.Error != null)
            {
                MessageDialog.Show(
                    this,
                    "Updater",
                    e.Cancelled ? "Cancelled" : "Error",
                    e.Cancelled ? "Update got cancelled" : "Please check the logs for more information.",
                    e.Cancelled ? SystemIcons.Warning : SystemIcons.Error,
                    MessageBoxButtons.OK);

                return;
            }

            if (!e.UpdateAvailable)
            {
                MessageDialog.Show(
                    this,
                    "Updater",
                    "No update available!",
                    $"You already have the latest version ({e.LatestVersion}).",
                    SystemIcons.Information,
                    MessageBoxButtons.OK);

                return;
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdatesToolStripMenuItem.Enabled = false;

            Updater.Instance.CheckForUpdatesAsync();
        }
    }
}

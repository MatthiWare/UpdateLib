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
            this.InvokeOnUI(p => checkForUpdatesToolStripMenuItem.Enabled = true);

            if (e.Cancelled)
            {
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdatesToolStripMenuItem.Enabled = false;

            Updater.Instance.CheckForUpdatesAsync();
        }
    }
}

using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;
using System;
using System.Drawing;
using System.Threading;
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
            updater.UpdateURL = "https://dl.dropboxusercontent.com/u/30635736/UpdateLib/Dev/updatefile.xml"; 
            updater.CheckForUpdatesCompleted += Updater_CheckForUpdatesCompleted;

            updater.Initialize();
        }

        private void Updater_CheckForUpdatesCompleted(object sender, CheckForUpdatesCompletedEventArgs e)
        {
            if (e.Cancelled)
                Logger.Debug(nameof(Updater), "Cancelled");

            if (e.Error != null)
                Logger.Error(nameof(Updater), e.Error);

            Logger.Debug(nameof(Updater), $"Version: {e.LatestVersion}");
            Logger.Debug(nameof(Updater), $"Update available: {e.UpdateAvailable}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = new MessageDialog(
                "Test title",
                "Version 1.0.0.0 available",
                "Download update now?\nPress yes to download or no to cancel.", SystemIcons.Question, MessageBoxButtons.YesNoCancel).ShowDialog(this);

            if (result == DialogResult.Yes)
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
            Action<object> test = new Action<object>((o) => {  });

            Func<int, bool> test2 = new Func<int, bool>((i) => { return i%2==0; });

            Console.WriteLine("Result: " + AsyncTaskFactory.StartNew<bool>(test2, 2).AwaitTask());

        }
    }
}

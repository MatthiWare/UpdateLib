using System;
using System.Text;
using MatthiWare.UpdateLib.Files;
using System.Windows.Forms;
using MatthiWare.UpdateLib.UI;
using System.Drawing;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Logging.Writers;
using MatthiWare.UpdateLib.Logging;
using System.Security;
using System.Diagnostics;

namespace MatthiWare.UpdateLib
{
    public class Updater
    {
        #region Singleton
        private static volatile Updater instance = null;
        private static readonly object synclock = new object();

        /// <summary>
        /// Gets a thread safe instance of <see cref="Updater"/> 
        /// </summary>
        public static Updater Instance
        {
            get
            {
                if (instance == null)
                    lock (synclock)
                        if (instance == null)
                            instance = new Updater();

                return instance;
            }
        }
        #endregion

        private int m_parentPid;

        public event EventHandler<CheckForUpdatesCompletedEventArgs> CheckForUpdatesCompleted;

        private string m_updateUrl = "";
        public string UpdateURL
        {
            get { return m_updateUrl; }
            set
            {
                m_updateUrl = value;
                RemoteBasePath = GetRemoteBasePath();
            }
        }

        public InstallationMode InstallationMode { get; set; } = InstallationMode.Shared;

        public bool EnableCmdArguments { get; set; } = true;
        public bool UpdateSilently { get; set; } = false;
        public string UpdateSilentlyCmdArg { get; set; } = "--silent";

        public string StartUpdatingCmdArg { get; set; } = "--update";

        public bool WaitForProcessExit { get; set; }
        public string WaitForProcessCmdArg { get; set; } = "--wait";

        public bool ShowUpdateMessage { get; set; } = true;
        public bool ShowMessageOnNoUpdate { get; set; } = true;
        public bool ShowErrorMessage { get; set; } = true;
        public PathVariableConverter Converter { get; private set; }

        public bool AllowUnsafeConnection { get; set; } = false;

        /// <summary>
        /// Gets the clean up task
        /// </summary>
        public CleanUpTask CleanUpTask { get; private set; }

        /// <summary>
        /// Gets the update cache task
        /// </summary>
        public UpdateCacheTask UpdateCacheTask { get; private set; }

        public bool IsInitialized { get; private set; }

        internal string RemoteBasePath { get; set; }

        #region Fluent API

        public Updater ConfigureUnsafeConnections(bool allow)
        {
            AllowUnsafeConnection = allow;

            return this;
        }

        public Updater ConfigureInstallationMode(InstallationMode mode)
        {
            InstallationMode = mode;

            return this;
        }

        public Updater ConfigureCmdArgs(bool enabled)
        {
            EnableCmdArguments = enabled;

            return this;
        }

        public Updater ConfigureSilentCmdArg(string cmdArg)
        {
            UpdateSilentlyCmdArg = cmdArg;

            return this;
        }

        public Updater ConfigureUpdateCmdArg(string cmdArg)
        {
            StartUpdatingCmdArg = cmdArg;

            return this;
        }

        public Updater ConfigureWaitForProcessCmdArg(string cmdArg)
        {
            WaitForProcessCmdArg = cmdArg;

            return this;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="Updater"/> with the default settings. 
        /// </summary>
        private Updater()
        {
            Converter = new PathVariableConverter();
        }

        /// <summary>
        /// Initializes the updater
        /// </summary>
        public void Initialize()
        {
            StartInitializationTasks();

            if (!EnableCmdArguments)
                return;

            bool shouldStartUpdating = ParseCmdArguments(Environment.GetCommandLineArgs());

            if (WaitForProcessExit)
                WaitForProcessToExit(m_parentPid);

            if (shouldStartUpdating)
                CheckForUpdates();
        }

        private void StartInitializationTasks()
        {
            CleanUpTask = new CleanUpTask(".");
            CleanUpTask.ConfigureAwait(false).Start();

            UpdateCacheTask = new UpdateCacheTask();
            UpdateCacheTask.ConfigureAwait(false).Start();

            IsInitialized = true;
        }

        private bool ParseCmdArguments(string[] args)
        {
            bool startUpdating = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == StartUpdatingCmdArg)
                    startUpdating = true;
                else if (args[i] == UpdateSilentlyCmdArg)
                    UpdateSilently = true;
                else if (args[i] == WaitForProcessCmdArg)
                    if (i + 1 <= args.Length && int.TryParse(args[i + 1], out m_parentPid))
                    {
                        i++;
                        WaitForProcessExit = true;
                    }

            }

            return startUpdating;
        }

        private void WaitForProcessToExit(int pid)
        {
            Process watchdog = Process.GetProcessById(pid);
            watchdog.CloseMainWindow();
            watchdog.WaitForExit();
        }

        /// <summary>
        /// Starting the update process
        /// </summary>
        /// <returns>Whether or not there is an update available and the latest version</returns>
        public CheckForUpdatesTask.Data CheckForUpdates()
        {
            return CheckForUpdatesAsync().AwaitTask().Result;
        }

        /// <summary>
        /// Starting the update process
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <returns>Whether or not there is an update available and the latest version</returns>
        public CheckForUpdatesTask.Data CheckForUpdates(IWin32Window owner)
        {
            return CheckForUpdatesAsync(owner).AwaitTask().Result;
        }

        /// <summary>
        /// Start the update process asynchronously
        /// </summary>
        /// <returns>The update checker task.</returns>
        public CheckForUpdatesTask CheckForUpdatesAsync()
        {
            return CheckForUpdatesAsync(null);
        }

        /// <summary>
        /// Start the update process asynchronously
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <returns>The update checker task.</returns>
        public CheckForUpdatesTask CheckForUpdatesAsync(IWin32Window owner)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The updater needs to be initialized first.");

            if (string.IsNullOrEmpty(UpdateURL))
                throw new ArgumentException("You need to specifify an update url", nameof(UpdateURL));

            if (!AllowUnsafeConnection && new Uri(UpdateURL).Scheme != Uri.UriSchemeHttps)
                throw new SecurityException("Using unsafe connections to update from is not allowed");

            CheckForUpdatesTask task = new CheckForUpdatesTask(UpdateURL);
            task.TaskCompleted += (o, e) =>
            {
                if (!task.Result.UpdateAvailable || e.Cancelled || e.Error != null)
                {
                    CheckForUpdatesCompleted?.Invoke(task, new CheckForUpdatesCompletedEventArgs(task.Result, e));
                    return;
                }

                DialogResult result = DialogResult.Yes;

                if (!UpdateSilently)
                    result = MessageDialog.Show(
                        "Update available",
                        $"Version {task.Result.Version} available",
                        "Update now?\nPress yes to update or no to cancel.",
                        SystemIcons.Question);


                if (result == DialogResult.Yes)
                {
                    if (UpdateSilently)
                    {
                        UpdateWithoutGUI(task.Result.UpdateFile);
                    }
                    else
                    {
                        UpdaterForm updateForm = new UpdaterForm(task.Result.UpdateFile);
                        updateForm.ShowDialog(owner);
                    }
                }

                CheckForUpdatesCompleted?.Invoke(task, new CheckForUpdatesCompletedEventArgs(task.Result, e));
            };
            return (CheckForUpdatesTask)task.Start();
        }

        /// <summary>
        /// Gets the cached index of the current application
        /// </summary>
        /// <returns>The <see cref="HashCacheFile"/> of the current application</returns>
        public HashCacheFile GetCache()
        {
            return UpdateCacheTask.AwaitTask().Result;
        }

        private string GetRemoteBasePath()
        {
            string[] tokens = UpdateURL.Split('/');
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < tokens.Length - 1; i++)
            {
                builder.Append(tokens[i]);
                builder.Append('/');
            }

            return builder.ToString();
        }

        private void UpdateWithoutGUI(UpdateFile file)
        {
            DownloadManager downloader = new DownloadManager(file);
            downloader.Update();
        }

    }
}

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
using System.Linq;
using MatthiWare.UpdateLib.Utils;
using System.Collections.Generic;
using System.Reflection;

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

        #region Fields

        private int m_parentPid;
        private string m_updateUrl = "";

        #endregion

        #region Events

        public event EventHandler<CheckForUpdatesCompletedEventArgs> CheckForUpdatesCompleted;

        #endregion

        #region Properties

        public string UpdateURL
        {
            get { return m_updateUrl; }
            set
            {
                m_updateUrl = value;
                RemoteBasePath = IOUtils.GetRemoteBasePath(value);
            }
        }
        public InstallationMode InstallationMode { get; set; } = InstallationMode.Shared;
        public bool StartUpdating { get; set; } = false;
        public bool UpdateSilently { get; set; } = false;
        public string UpdateSilentlyCmdArg { get; set; } = "--silent";
        public string StartUpdatingCmdArg { get; set; } = "--update";
        public bool WaitForProcessExit { get; set; }
        public string WaitForProcessCmdArg { get; set; } = "--wait";
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

        #endregion

        #region Fluent API

        /// <summary>
        /// Configures if unsafe connections are allowed
        /// </summary>
        /// <remarks>Do not enable this unless you know what you are doing</remarks>
        /// <param name="allow">Allowed?</param>
        /// <returns><see cref="Updater"/></returns>
        public Updater ConfigureUnsafeConnections(bool allow)
        {
            AllowUnsafeConnection = allow;

            return this;
        }

        /// <summary>
        /// Configures the installation mode for the client
        /// </summary>
        /// <param name="mode">The <see cref="InstallationMode"/></param>
        /// <returns><see cref="Updater"/></returns>
        public Updater ConfigureInstallationMode(InstallationMode mode)
        {
            InstallationMode = mode;

            return this;
        }

        /// <summary>
        /// Configures the update silently command switch
        /// </summary>
        /// <param name="cmdArg">Command name</param>
        /// <returns><see cref="Updater"/></returns>
        public Updater ConfigureSilentCmdArg(string cmdArg)
        {
            UpdateSilentlyCmdArg = cmdArg;

            return this;
        }

        /// <summary>
        /// Configures the update command switch
        /// </summary>
        /// <param name="cmdArg">Command name</param>
        /// <returns><see cref="Updater"/></returns>
        public Updater ConfigureUpdateCmdArg(string cmdArg)
        {
            StartUpdatingCmdArg = cmdArg;

            return this;
        }

        /// <summary>
        /// Configures the wait for process to exit command switch
        /// </summary>
        /// <param name="cmdArg">Command name</param>
        /// <returns><see cref="Updater"/></returns>
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

            ParseCmdArguments(Environment.GetCommandLineArgs());

            if (WaitForProcessExit)
                WaitForProcessToExit(m_parentPid);

            if (StartUpdating)
                CheckForUpdates();
        }

        /// <summary>
        /// Starts the initialization tasks
        /// </summary>
        private void StartInitializationTasks()
        {
            CleanUpTask = new CleanUpTask(".");
            CleanUpTask.ConfigureAwait(false).Start();

            UpdateCacheTask = new UpdateCacheTask();
            UpdateCacheTask.ConfigureAwait(false).Start();

            IsInitialized = true;
        }

        /// <summary>
        /// Parses the given arguments
        /// </summary>
        /// <param name="args">The arguments to parse</param>
        private void ParseCmdArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(StartUpdatingCmdArg) && args[i] == StartUpdatingCmdArg)
                    StartUpdating = true;
                else if (!string.IsNullOrEmpty(UpdateSilentlyCmdArg) && args[i] == UpdateSilentlyCmdArg)
                    UpdateSilently = true;
                else if (!string.IsNullOrEmpty(WaitForProcessCmdArg) && args[i] == WaitForProcessCmdArg)
                    if (i + 1 <= args.Length && int.TryParse(args[i + 1], out m_parentPid))
                    {
                        i++;
                        WaitForProcessExit = true;
                    }
            }
        }

        /// <summary>
        /// Waits for a process to exit on the current thread
        /// </summary>
        /// <param name="pid">Process ID</param>
        private void WaitForProcessToExit(int pid)
        {
            Process[] processes = Process.GetProcesses();
            Process toWatch = processes.FirstOrDefault(p => p.Id == pid);

            if (toWatch == null) return;

            toWatch.CloseMainWindow();
            toWatch.WaitForExit();
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
            if (!IsInitialized) throw new InvalidOperationException("Updater has not been initialized yet!");

            return UpdateCacheTask.AwaitTask().Result;
        }

        /// <summary>
        /// Updates without user interaction
        /// </summary>
        /// <param name="file">The update specifications file <see cref="UpdateFile"/> </param>
        private void UpdateWithoutGUI(UpdateFile file)
        {
            DownloadManager downloader = new DownloadManager(file);
            downloader.Update();
        }

        internal void RestartApp()
        {
            List<string> args = new List<string>(Environment.GetCommandLineArgs());

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i] == StartUpdatingCmdArg || args[i] == UpdateSilentlyCmdArg)
                {
                    args[i] = string.Empty;
                }
                else if (args[i] == WaitForProcessCmdArg)
                {
                    args[i] = string.Empty;
                    if (i + 1 < args.Count)
                        args[++i] = string.Empty;
                }
            }

            args.Add(instance.WaitForProcessCmdArg);
            args.Add(Process.GetCurrentProcess().Id.ToString());

            string arguments = args.Where(a => !string.IsNullOrEmpty(a)).Distinct().AppendAll(" ");

            ProcessStartInfo startInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, arguments);
            startInfo.UseShellExecute = false;
            Process.Start(startInfo);

            Environment.Exit(0);
        }

    }
}

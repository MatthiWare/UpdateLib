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
using System.IO;

namespace MatthiWare.UpdateLib
{
    public sealed class Updater
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

        private int m_pid;
        private string m_updateUrl = "";
        private string m_argUpdateSilent = "--silent";
        private string m_argUpdate = "--update";
        private string m_argWait = "--wait";
        private Lazy<PathVariableConverter> m_lazyPathVarConv = new Lazy<PathVariableConverter>(() => new PathVariableConverter());
        private TimeSpan m_cacheInvalidation = TimeSpan.FromMinutes(5);
        private Lazy<Logger> m_lazyLogger = new Lazy<Logger>(() => new Logger());

        private static Lazy<string> m_lazyProductName = new Lazy<string>(() =>
        {
            AssemblyProductAttribute attr = Attribute.GetCustomAttribute(Assembly.GetAssembly(typeof(Updater)), typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
            return attr?.Product ?? "UpdateLib";
        });

        #endregion

        #region Events

        /// <summary>
        /// Check for updates completed event.
        /// </summary>
        public event EventHandler<CheckForUpdatesCompletedEventArgs> CheckForUpdatesCompleted;

        #endregion

        #region Properties

        internal static string ProductName { get { return m_lazyProductName.Value; } }

        /// <summary>
        /// Gets or sets the url to update from
        /// </summary>
        /// <remarks>If you want to specify an unsafe connection you should enable <see cref="AllowUnsafeConnection"/></remarks>
        public string UpdateURL
        {
            get { return m_updateUrl; }
            set
            {
                m_updateUrl = value;
                RemoteBasePath = IOUtils.GetRemoteBasePath(value);
            }
        }

        public bool UpdateRequiresAdmin { get; set; } = true;

        /// <summary>
        /// Gets the logger for the application.
        /// </summary>
        public ILogger Logger { get { return m_lazyLogger.Value; } }

        /// <summary>
        /// Gets or sets the Updater Installation mode
        /// </summary>
        public InstallationMode InstallationMode { get; set; } = InstallationMode.Shared;

        /// <summary>
        /// Gets or sets if we want to check for updates before the actual program is loaded.
        /// </summary>
        public bool StartUpdating { get; set; } = false;

        /// <summary>
        /// Gets or sets if we want to update silently (no UI interaction).
        /// </summary>
        public bool UpdateSilently { get; set; } = false;

        /// <summary>
        /// Gets or sets the command line argument for the silent switch
        /// If this argument has been set and is passed in the command line it will set <see cref="UpdateSilently"/> to <c>True</c> 
        /// </summary>
        public string UpdateSilentlyCmdArg
        {
            get { return m_argUpdateSilent; }
            set { SetAndVerifyCmdArgument(ref m_argUpdateSilent, value); }
        }

        /// <summary>
        /// Gets or sets the command line argument for the update switch
        /// If this argument has been set and is passed in the command line it will set <see cref="StartUpdating"/> to <c>True</c> 
        /// </summary>
        public string StartUpdatingCmdArg
        {
            get { return m_argUpdate; }
            set { SetAndVerifyCmdArgument(ref m_argUpdate, value); }
        }

        /// <summary>
        /// Gets or sets the command line argument for the wait switch
        /// If this argument has been set and passed in the command line followed by an <see cref="Process.Id"/> it will set <see cref="WaitForProcessExit"/> to <c>True</c>
        /// </summary>
        public string WaitForProcessCmdArg
        {
            get { return m_argWait; }
            set { SetAndVerifyCmdArgument(ref m_argWait, value); }
        }

        /// <summary>
        /// Indicates if we want to wait for the given process to wait
        /// See <seealso cref="WaitForProcessCmdArg"/> and <seealso cref="ConfigureWaitForProcessCmdArg(string)"/>
        /// If this property has been set to true it will wait for the <see cref="Process.Id"/> to exit before continuing when <see cref="Initialize"/> has been called.  
        /// </summary>
        public bool WaitForProcessExit { get; set; }

        /// <summary>
        /// Gets the <see cref="PathVariableConverter"/>.
        /// This property is only initialized when called.
        /// </summary>
        public PathVariableConverter Converter
        {
            get { return m_lazyPathVarConv.Value; }
            private set { m_lazyPathVarConv.Value = value; }
        }

        /// <summary>
        /// Gets or sets if the updater allows unsafe connection 
        /// <value>`True` to allow HTTP connections, `False` to only allow HTTPS connections</value>
        /// </summary>
        public bool AllowUnsafeConnection { get; set; } = false;

        /// <summary>
        /// Gets the clean up task
        /// </summary>
        public CleanUpTask CleanUpTask { get; private set; }

        /// <summary>
        /// Gets the update cache task
        /// </summary>
        public UpdateCacheTask UpdateCacheTask { get; private set; }

        /// <summary>
        /// Is the updater already initialized?
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Time before all the cached files become invalid
        /// </summary>
        public TimeSpan CacheInvalidationTime
        {
            get { return m_cacheInvalidation; }
            set { m_cacheInvalidation = value; }
        }

        public bool NeedsRestartBeforeUpdate { get; set; } = true;

        /// <summary>
        /// The remote base path to use for downloading etc..
        /// </summary>
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

        public Updater ConfigureLogger(Action<ILogger> logAction)
        {
            logAction(Logger);

            return this;
        }

        public Updater ConfigureNeedsRestartBeforeUpdate(bool needsRestartBeforeUpdate)
        {
            NeedsRestartBeforeUpdate = needsRestartBeforeUpdate;

            return this;
        }

        public Updater ConfigureCacheInvalidation(TimeSpan timeTillInvalidation)
        {
            CacheInvalidationTime = timeTillInvalidation;

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

        public Updater ConfigureUpdateNeedsAdmin(bool needsAdmin)
        {
            UpdateRequiresAdmin = needsAdmin;

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

        /// <summary>
        /// Configures the update url
        /// </summary>
        /// <remarks>To use HTTP you should enable <see cref="AllowUnsafeConnection"/> </remarks>
        /// <param name="url">Url to update from</param>
        /// <returns><see cref="Updater"/> </returns>
        public Updater ConfigureUpdateUrl(string url)
        {
            UpdateURL = url;

            return this;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="Updater"/> with the default settings. 
        /// </summary>
        private Updater() { }

        /// <summary>
        /// Initializes the updater
        /// </summary>
        public void Initialize()
        {
            StartInitializationTasks();

            ParseCmdArguments(Environment.GetCommandLineArgs());

            if (WaitForProcessExit)
                WaitForProcessToExit(m_pid);

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
                else if (!string.IsNullOrEmpty(WaitForProcessCmdArg) && args[i] == WaitForProcessCmdArg && i + 1 < args.Length && int.TryParse(args[++i], out m_pid))
                    WaitForProcessExit = true;
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
                bool error = e.Error != null;
                bool cancelled = e.Cancelled;
                bool update = task.Result.UpdateAvailable;

                if (!update || cancelled || error)
                {
                    if (error)
                        Logger.Error(GetType().Name, e.Error);

                    if (!update)
                        Logger.Info()

                    if (!UpdateSilently)
                        MessageDialog.Show(
                           owner,
                           $"{ProductName} Updater",
                           error ? "Error while updating" : (cancelled ? "Cancelled" : "No Update available"),
                           error ? "Check the log files for more information!" : (cancelled ? "Update got cancelled" : $"You already have the latest version {task.Result.Version}"),
                           error ? SystemIcons.Error : (cancelled ? SystemIcons.Warning : SystemIcons.Information),
                           MessageBoxButtons.OK);

                    CheckForUpdatesCompleted?.Invoke(task, new CheckForUpdatesCompletedEventArgs(task.Result, e));
                    return;
                }

                DialogResult result = DialogResult.Yes;

                if (!UpdateSilently && !StartUpdating)
                    result = MessageDialog.Show(
                        owner,
                        "Update available",
                        $"Version {task.Result.Version} available",
                        "Update now?\nPress yes to update or no to cancel.",
                        SystemIcons.Question);

                if (result == DialogResult.Yes)
                {
                    if (!StartUpdating && NeedsRestartBeforeUpdate)
                        RestartApp(true, UpdateSilently, true, UpdateRequiresAdmin);

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

        internal void RestartApp(bool update = false, bool silent = false, bool waitForPid = true, bool asAdmin = false)
        {
            List<string> args = new List<string>(Environment.GetCommandLineArgs());

            for (int i = 0; i < args.Count; i++)
            {
                if ((!update && args[i] == StartUpdatingCmdArg) || (!silent && args[i] == UpdateSilentlyCmdArg))
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

            if (waitForPid && !string.IsNullOrEmpty(instance.WaitForProcessCmdArg))
            {
                args.Add(instance.WaitForProcessCmdArg);
                args.Add(Process.GetCurrentProcess().Id.ToString());
            }

            if (update && !string.IsNullOrEmpty(instance.StartUpdatingCmdArg) && !args.Contains(instance.StartUpdatingCmdArg))
                args.Add(instance.StartUpdatingCmdArg);

            if (silent && !string.IsNullOrEmpty(instance.UpdateSilentlyCmdArg) && !args.Contains(instance.UpdateSilentlyCmdArg))
                args.Add(instance.UpdateSilentlyCmdArg);

            string arguments = args.Where(a => !string.IsNullOrEmpty(a)).Distinct().AppendAll(" ");

            ProcessStartInfo startInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, arguments);

            startInfo.UseShellExecute = true;

            if (asAdmin)
                startInfo.Verb = "runas";

            Process.Start(startInfo);

            Environment.Exit(0);
        }

        private void SetAndVerifyCmdArgument(ref string reference, string value)
        {
            if (value.Contains(' '))
                throw new ArgumentException("Command line argument can not contain spaces");

            reference = value;
        }
    }
}

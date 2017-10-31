/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using MatthiWare.UpdateLib.Files;
using System.Windows.Forms;
using MatthiWare.UpdateLib.UI;
using System.Drawing;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Logging;
using System.Security;
using System.Diagnostics;
using System.Linq;
using MatthiWare.UpdateLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using MatthiWare.UpdateLib.Security;
using System.ComponentModel;
using MatthiWare.UpdateLib.Common;

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

        private const string m_strUpdateLib = "UpdateLib";
        
        private string m_updateUrl = string.Empty;
        private const string m_argUpdateSilent = "silent";
        private const string m_argUpdate = "update";
        private const string m_argWait = "wait";
        private Lazy<PathVariableConverter> m_lazyPathVarConv = new Lazy<PathVariableConverter>(() => new PathVariableConverter());
        private TimeSpan m_cacheInvalidation = TimeSpan.FromMinutes(5);
        private Lazy<Logger> m_lazyLogger = new Lazy<Logger>(() => new Logger());
        private InstallationMode m_installationMode = InstallationMode.Shared;

        private static Lazy<string> m_lazyProductName = new Lazy<string>(() =>
        {
            AssemblyProductAttribute attr = Assembly.GetEntryAssembly()?.GetCustomAttributes(typeof(AssemblyProductAttribute), true).FirstOrDefault() as AssemblyProductAttribute;

            //AssemblyProductAttribute attr = Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), ) as AssemblyProductAttribute;
            return attr?.Product ?? m_strUpdateLib;
        });

        private static Lazy<string> m_lazyUpdaterName = new Lazy<string>(() =>
        {
            AssemblyProductAttribute attr = Assembly.GetAssembly(typeof(Updater))?.GetCustomAttributes(typeof(AssemblyProductAttribute), true).FirstOrDefault() as AssemblyProductAttribute;

            //AssemblyProductAttribute attr = Attribute.GetCustomAttribute(), typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
            return attr?.Product ?? m_strUpdateLib;
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

        internal static string UpdaterName { get { return m_lazyUpdaterName.Value; } }

        public CmdLineParser CommandLine { get; } = new CmdLineParser();

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

        /// <summary>
        /// Gets the logger for the application.
        /// </summary>
        public ILogger Logger { get { return m_lazyLogger.Value; } }

        /// <summary>
        /// Gets or sets the Updater Installation mode
        /// </summary>
        public InstallationMode InstallationMode
        {
            get { return m_installationMode; }
            set
            {
                if (m_installationMode != value)
                {
                    m_installationMode = value;
                    IOUtils.ReinitializeAppData();
                }
            }
        }


        /// <summary>
        /// Gets or sets if we want to check for updates before the actual program is loaded.
        /// </summary>
        public bool StartUpdating { get; private set; } = false;

        /// <summary>
        /// Gets or sets if we want to update silently (no UI interaction).
        /// </summary>
        public bool UpdateSilently { get; private set; } = false;

        /// <summary>
        /// Indicates if we want to wait for the given process to wait
        /// See <seealso cref="WaitForProcessCmdArg"/> and <seealso cref="ConfigureWaitForProcessCmdArg(string)"/>
        /// If this property has been set to true it will wait for the <see cref="Process.Id"/> to exit before continuing when <see cref="Initialize"/> has been called.  
        /// </summary>
        public bool WaitForProcessExit { get; private set; }

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

        /// <summary>
        /// Gets or sets if the updater needs to launch as a new instance.
        /// <c>True</c> if you want cold-swapping, <c>False</c> if you want hot-swapping
        /// </summary>
        /// <remarks>Hot-swapping might cause issues if the files are still in use.</remarks>
        public bool NeedsRestartBeforeUpdate { get; set; } = true;

        /// <summary>
        /// The remote base path to use for downloading etc..
        /// </summary>
        internal string RemoteBasePath { get; set; }

        #endregion

        #region Fluent API

        /// <summary>
        /// Configures the path variable converter
        /// </summary>
        /// <param name="action">the action to perform on the <see cref="PathVariableConverter"/> </param>
        /// <returns><see cref="Updater"/> </returns>
        public Updater ConfigurePathConverter(Action<PathVariableConverter> action)
        {
            action(Converter);

            return this;
        }

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
        /// Configures the logger
        /// </summary>
        /// <param name="action">Action to perform on the logger</param>
        /// <returns><see cref="Updater"/> </returns>
        public Updater ConfigureLogger(Action<ILogger> action)
        {
            action(Logger);

            return this;
        }

        /// <summary>
        /// Configures the command line parser
        /// </summary>
        /// <param name="action">Action to perform on the command line parser</param>
        /// <returns><see cref="Updater"/> </returns>
        public Updater ConfigureCommandLineParser(Action<CmdLineParser> action)
        {
            action(CommandLine);

            return this;
        }

        /// <summary>
        /// Configures if the updater needs a restart before updating
        /// </summary>
        /// <remarks>Disabling this feature will allow for hot-swapping of the files. </remarks>
        /// <param name="needsRestartBeforeUpdate">Restart updater in new instance</param>
        /// <returns><see cref="Updater"/> </returns>
        public Updater ConfigureNeedsRestartBeforeUpdate(bool needsRestartBeforeUpdate)
        {
            NeedsRestartBeforeUpdate = needsRestartBeforeUpdate;

            return this;
        }

        /// <summary>
        /// Configures the time till the cache becomes invalid
        /// </summary>
        /// <param name="timeTillInvalidation">Specify the validity time</param>
        /// <returns><see cref="Updater"/> </returns>
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
        private Updater()
        {
            CommandLine.AddParameter(m_argUpdateSilent);
            CommandLine.AddParameter(m_argUpdate);
            CommandLine.AddParameter(m_argWait, ParamMandatoryType.Optional, ParamValueType.Int);
        }

        /// <summary>
        /// Initializes the updater
        /// </summary>
        public void Initialize()
        {
            StartInitializationTasks();

            // parse the command line
            CommandLine.Parse();


            WaitForProcessExit = CommandLine[m_argWait]?.IsFound ?? false;
            StartUpdating = CommandLine[m_argUpdate]?.IsFound ?? false;
            UpdateSilently = CommandLine[m_argUpdateSilent]?.IsFound ?? false;

            if (WaitForProcessExit)
                WaitForProcessToExit((int)CommandLine[m_argWait].Value);

            IsInitialized = true;

            if (StartUpdating)
                CheckForUpdates();
        }

        /// <summary>
        /// Starts the initialization tasks
        /// </summary>
        private void StartInitializationTasks()
        {
            CleanUpTask = new CleanUpTask("%appdir%");
            CleanUpTask.ConfigureAwait(false).Start();

            UpdateCacheTask = new UpdateCacheTask();
            UpdateCacheTask.ConfigureAwait(false).Start();
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
        public CheckForUpdatesTask.CheckForUpdatesResult CheckForUpdates()
        {
            return CheckForUpdatesAsync().AwaitTask().Result;
        }

        /// <summary>
        /// Starting the update process
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <returns>Whether or not there is an update available and the latest version</returns>
        public CheckForUpdatesTask.CheckForUpdatesResult CheckForUpdates(IWin32Window owner)
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
                bool adminReq = task.Result.AdminRightsNeeded;

                CheckForUpdatesCompleted?.Invoke(task, new CheckForUpdatesCompletedEventArgs(task.Result, e));

                if (!update || cancelled || error)
                {
                    if (error)
                        HandleException(owner, e.Error);
                    else if (cancelled)
                        HandleUserCancelled(owner);
                    else if (!update)
                        HandleNoUpdate(owner, task.Result.Version);

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

                if (result != DialogResult.Yes)
                    return;

                if ((!StartUpdating && NeedsRestartBeforeUpdate)
                    || (adminReq && !PermissionUtil.IsProcessElevated))
                    if (!RestartApp(true, UpdateSilently, true, adminReq))
                        return;

                if (UpdateSilently)
                    UpdateWithoutGUI(task.Result.UpdateFile);
                else
                {
                    UpdaterForm updateForm = new UpdaterForm(task.Result.UpdateFile);
                    updateForm.ShowDialog(owner);
                }



            };

            return (CheckForUpdatesTask)task.Start();
        }

        private void HandleException(IWin32Window owner, Exception e)
        {
            if (UpdateSilently)
                return;

            if (e is WebException)
                MessageDialog.Show(
                   owner,
                   $"{ProductName} Updater",
                   "Error while updating",
                   "Unable to connect to the update server!\nPlease check your internet connection!",
                    SystemIcons.Warning,
                   MessageBoxButtons.OK);
            else if (e is Win32Exception)
                MessageDialog.Show(
                   owner,
                   $"{ProductName} Updater",
                   "Update cancelled",
                   "Update got cancelled by the user!",
                    SystemIcons.Warning,
                   MessageBoxButtons.OK);
            else
                MessageDialog.Show(
                   owner,
                   $"{ProductName} Updater",
                   "Error while updating",
                   "Check the log files for more information!",
                   SystemIcons.Error,
                   MessageBoxButtons.OK);
        }

        private void HandleNoUpdate(IWin32Window owner, string latest)
        {
            Logger.Info(nameof(Updater), nameof(CheckForUpdatesAsync), "No update available");

            if (!UpdateSilently)
                MessageDialog.Show(
                   owner,
                   $"{ProductName} Updater",
                   "No Update available",
                   $"You already have the latest version {latest}",
                   SystemIcons.Information,
                   MessageBoxButtons.OK);
        }

        private void HandleUserCancelled(IWin32Window owner)
        {
            Logger.Info(nameof(Updater), nameof(CheckForUpdatesAsync), "Update cancalled");

            if (!UpdateSilently)
                MessageDialog.Show(
                   owner,
                   $"{ProductName} Updater",
                    "Cancelled",
                    "Update got cancelled",
                   SystemIcons.Warning,
                   MessageBoxButtons.OK);
        }

        /// <summary>
        /// Gets the cached index of the current application
        /// </summary>
        /// <returns>The <see cref="HashCacheFile"/> of the current application</returns>
        public HashCacheFile GetCache()
        {
            return UpdateCacheTask.AwaitTask().Result;
        }

        /// <summary>
        /// Updates without user interaction
        /// </summary>
        /// <param name="file">The update specifications file <see cref="UpdateFile"/> </param>
        private void UpdateWithoutGUI(UpdateFile file)
        {
            DownloadManager downloader = new DownloadManager(file);
            downloader.Completed += (o, e) =>
            {
                GetCache().Save();
                RestartApp();
            };

            downloader.Update();
        }

        internal bool RestartApp(bool update = false, bool silent = false, bool waitForPid = true, bool asAdmin = false)
        {
            Logger.Info(nameof(Updater), nameof(RestartApp), $"Restarting app: update={update} silent={silent} waitForPid={waitForPid} asAdmin={asAdmin}");

            List<string> args = new List<string>(Environment.GetCommandLineArgs());

            for (int i = 0; i < args.Count; i++)
            {
                if ((!update && args[i] == CommandLine.ParameterPrefix + m_argUpdate) || (!silent && args[i] == CommandLine.ParameterPrefix + m_argUpdateSilent))
                {
                    args[i] = string.Empty;
                }
                else if (args[i] == CommandLine.ParameterPrefix + m_argWait)
                {
                    args[i] = string.Empty;
                    if (i + 1 < args.Count)
                        args[++i] = string.Empty;
                }
            }

            if (waitForPid && !args.Contains(CommandLine.ParameterPrefix + m_argWait))
            {
                args.Add(CommandLine.ParameterPrefix + m_argWait);
                args.Add(Process.GetCurrentProcess().Id.ToString());
            }

            if (update  && !args.Contains(CommandLine.ParameterPrefix + m_argUpdate))
                args.Add(CommandLine.ParameterPrefix + m_argUpdate);

            if (silent && !args.Contains(CommandLine.ParameterPrefix + m_argUpdateSilent))
                args.Add(CommandLine.ParameterPrefix + m_argUpdateSilent);

            string arguments = args.NotEmpty().Distinct().AppendAll(" ");

            ProcessStartInfo startInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, arguments);

            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = true;

            if (asAdmin)
                startInfo.Verb = "runas";

            try
            {
                Process proc = Process.Start(startInfo);

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Logger.Error(nameof(Updater), nameof(RestartApp), e);

                HandleException(null, e);

                return false;
            }

            // we will never reach this part of the code
            return true;
        }
    }
}

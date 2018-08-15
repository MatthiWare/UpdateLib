/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: Updater.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2015 - MatthiWare
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
 *  along with this program.  If not, see <https://github.com/MatthiWare/UpdateLib/blob/master/LICENSE>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;

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
        private const string m_argUpdateSilent = "silent";
        private const string m_argUpdate = "update";
        private const string m_argWait = "wait";
        private const string m_rollback = "rollback";
        private Lazy<PathVariableConverter> m_lazyPathVarConv = new Lazy<PathVariableConverter>(() => new PathVariableConverter());
        private InstallationMode m_installationMode = InstallationMode.Shared;

        #endregion

        #region Events

        /// <summary>
        /// Check for updates completed event.
        /// </summary>
        //public event EventHandler<CheckForUpdatesCompletedEventArgs> CheckForUpdatesCompleted;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command line parser. Use this to add additional command line arguments that need to be parsed. 
        /// </summary>
        public CmdLineParser CommandLine { get; } = new CmdLineParser();

        /// <summary>
        /// Gets the collection of Url's to update from
        /// </summary>
        /// <remarks>If you want to specify an unsafe connection you should enable <see cref="AllowUnsafeConnection"/></remarks>
        public IList<string> UpdateURLs { get; } = new List<string>();

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

        public bool Rollback { get; private set; }

        /// <summary>
        /// Gets the <see cref="PathVariableConverter"/>.
        /// This property is only initialized when called.
        /// </summary>
        public PathVariableConverter Converter
        {
            get { return m_lazyPathVarConv.Value; }
        }

        /// <summary>
        /// Gets or sets if the updater allows unsafe connection 
        /// <value>`True` to allow HTTP connections, `False` to only allow HTTPS connections</value>
        /// </summary>
        public bool AllowUnsafeConnection { get; set; } = false;

        ///// <summary>
        ///// Gets the clean up task
        ///// </summary>
        //public CleanUpTask CleanUpTask { get; private set; }

        ///// <summary>
        ///// Gets the update cache task
        ///// </summary>
        //public UpdateCacheTask UpdateCacheTask { get; private set; }



        /// <summary>
        /// Is the updater already initialized?
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets if the updater needs to launch as a new instance.
        /// <c>True</c> if you want cold-swapping, <c>False</c> if you want hot-swapping
        /// </summary>
        /// <remarks>Hot-swapping might cause issues if the files are still in use.</remarks>
        public bool NeedsRestartBeforeUpdate { get; set; } = true;

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
        public Updater ConfigureAllowUnsafeConnections(bool allow)
        {
            AllowUnsafeConnection = allow;

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
        [Obsolete("No longer used")]
        public Updater ConfigureCacheInvalidation(TimeSpan timeTillInvalidation)
        {
            //CacheInvalidationTime = timeTillInvalidation;

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
        /// <param name="uri">Uri to update from</param>
        /// <returns><see cref="Updater"/> </returns>
        public Updater ConfigureAddUpdateUri(string uri)
        {
            UpdateURLs.Add(uri);

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
            CommandLine.AddParameter(m_rollback);
        }

        /// <summary>
        /// Initializes the updater
        /// </summary>
        public void Initialize()
        {
            StartInitializationTasks();

            // parse the command line
            CommandLine.Parse();

            // Set cmd line flags
            WaitForProcessExit = CommandLine[m_argWait]?.IsFound ?? false;
            StartUpdating = CommandLine[m_argUpdate]?.IsFound ?? false;
            UpdateSilently = CommandLine[m_argUpdateSilent]?.IsFound ?? false;
            Rollback = CommandLine[m_rollback]?.IsFound ?? false;

            if (WaitForProcessExit) WaitForProcessToExit((int)CommandLine[m_argWait].Value);

            IsInitialized = true;

            //if (StartUpdating) CheckForUpdates();
        }

        /// <summary>
        /// Starts the initialization tasks
        /// </summary>
        private void StartInitializationTasks()
        {

        }

        /// <summary>
        /// Waits for a process to exit on the current thread
        /// </summary>
        /// <param name="pid">Process ID</param>
        private void WaitForProcessToExit(int pid)
        {
            var process = Process.GetProcesses().FirstOrDefault(p => p.Id == pid);

            process?.CloseMainWindow();
            process?.WaitForExit();
        }

        /// <summary>
        /// Gets the cache of the updater
        /// </summary>
        /// <returns>The loaded <see cref="CacheFile"/> of the current application</returns>
        //public CacheFile GetCache() => m_loadCacheTask.AwaitTask().Result;

        /// <summary>
        /// Updates without user interaction
        /// </summary>
        /// <param name="updateInfo">The update specifications file <see cref="UpdateInfo"/> </param>
        private void UpdateWithoutGUI(UpdateInfo updateInfo, IList<string> urls)
        {
            //var downloadManager = new DownloadManager(updateInfo, urls);

            //downloadManager.Completed += (o, e) =>
            //{
            //    //GetCache2().Save();
            //    RestartApp();
            //};

            //downloadManager.Download();
        }

        internal bool RestartApp(bool update = false, bool silent = false, bool waitForPidExit = true, bool asAdmin = false)
        {
            //Logger.Debug(nameof(Updater), nameof(RestartApp), $"Restarting app: [update={update}; silent={silent}; waitForPidExit={waitForPidExit}; asAdmin={asAdmin}]");

            var args = new List<string>(Environment.GetCommandLineArgs());

            var cmdUpdate = $"{CommandLine.ParameterPrefix}{m_argUpdate}";
            var cmdSilent = $"{CommandLine.ParameterPrefix}{m_argUpdateSilent}";
            var cmdWait = $"{CommandLine.ParameterPrefix}{m_argWait}";

            for (int i = 0; i < args.Count; i++)
            {
                var carg = args[i];

                if ((!update && carg == cmdUpdate) || (!silent && carg == cmdSilent))
                {
                    args[i] = string.Empty;
                }
                else if (carg == cmdWait)
                {
                    args[i] = string.Empty;
                    if (i + 1 < args.Count)
                        args[++i] = string.Empty;
                }
            }

            if (waitForPidExit && !args.Contains(cmdWait))
            {
                args.Add(cmdWait);
                args.Add(Process.GetCurrentProcess().Id.ToString());
            }

            if (update && !args.Contains(cmdUpdate))
                args.Add(cmdUpdate);

            if (silent && !args.Contains(cmdSilent))
                args.Add(cmdSilent);

            string arguments = args.NotEmpty().Distinct().AppendAll(" ");

            ProcessStartInfo startInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, arguments)
            {
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = true,
                Verb = (asAdmin) ? "runas" : string.Empty
            };

            try
            {
                Process.Start(startInfo);

                // gracefully exit the current process
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                // Logger.Error(nameof(Updater), nameof(RestartApp), e);

                return false;
            }

            // we will never reach this part of the code
            return true;
        }
    }
}

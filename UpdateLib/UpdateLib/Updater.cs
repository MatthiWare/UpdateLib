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
using System.Threading;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Abstractions.Internal;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Core;
using MatthiWare.UpdateLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatthiWare.UpdateLib
{
    public sealed class Updater : IUpdater
    {
        #region Fields

        private readonly ILogger logger;
        private readonly UpdateLibOptions options;
        private readonly ICommandLineParser cmd;
        private readonly IUpdateManager updateManager;

        private bool m_initialized = false;

        /// <summary>
        /// Gets or sets if we want to check for updates before the actual program is loaded.
        /// </summary>
        private bool m_startUpdating = false;

        /// <summary>
        /// Indicates if we want to wait for the given process to wait
        /// If this property has been set to true it will wait for the <see cref="Process.Id"/> to exit before continuing when <see cref="InitializeAsync"/> has been called.  
        /// </summary>
        private bool m_waitForProcessExit = false;

        private bool m_rollback = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="Updater"/> with the default settings. 
        /// </summary>
        public Updater(ILogger<Updater> logger, IOptions<UpdateLibOptions> options, ICommandLineParser commandLineParser, IUpdateManager updateManager)
        {
            this.logger = logger;
            this.options = options.Value;
            cmd = commandLineParser;
            this.updateManager = updateManager;

            //CommandLine.AddParameter(m_argUpdateSilent);
            //CommandLine.AddParameter(m_argUpdate);
            //CommandLine.AddParameter(m_argWait, ParamMandatoryType.Optional, ParamValueType.Int);
            //CommandLine.AddParameter(m_rollback);
        }

        public static UpdaterBuilder GetBuilder()
            => UpdaterBuilder.CreateDefaultUpdateBuilder();

        /// <summary>
        /// Initializes the updater
        /// </summary>
        public async Task InitializeAsync(CancellationToken cancellation = default)
        {
            //StartInitializationTasks();

            // parse the command line
            cmd.Parse();

            // Set cmd line flags
            m_waitForProcessExit = cmd.Get(options.WaitArgumentName)?.IsFound ?? false;
            m_startUpdating = cmd.Get(options.UpdateArgumentName)?.IsFound ?? false;
            m_rollback = cmd.Get(options.RollbackArgumentName)?.IsFound ?? false;

            if (m_waitForProcessExit) await WaitForProcessToExitAsync(cmd.Get<int>(options.WaitArgumentName).Value, cancellation);

            m_initialized = true;

            if (m_startUpdating) await CheckForUpdatesAsync(cancellation);
        }

        public async Task<CheckForUpdatesResult> CheckForUpdatesAsync(CancellationToken cancellation = default)
        {
            var childCts = CancellationTokenSource.CreateLinkedTokenSource(cancellation);

            if (!m_initialized) throw new InvalidOperationException("The updater needs to be initialized first");
            if (string.IsNullOrEmpty(options.UpdateUrl)) throw new ArgumentException("No update url specified", nameof(UpdateLibOptions));

            var checkForUpdatesTask = updateManager.CheckForUpdatesAsync(childCts.Token);
            var result = await checkForUpdatesTask;

            if (checkForUpdatesTask.IsCanceled || checkForUpdatesTask.IsFaulted || !result.UpdateAvailable)
            {
                // TODO: what to do here? Let the caller handle it?
                return result;
            }

            // update available yay!!
            // TODO: let the caller decide if we want to update with some middleware?
            Func<bool> allowedToUpdate = new Func<bool>(() => true);

            if (!allowedToUpdate())
            {
                // TODO: Is this a good way?
                childCts.Cancel();
            }

            bool elevated = false;

            // we are allowed to update
            if (((!m_startUpdating || (result.AdminRightsNeeded && !elevated))) && !RestartApp(true, true, result.AdminRightsNeeded))
                return result;

            await updateManager.UpdateAsync(childCts.Token);

            return result;
        }

        /// <summary>
        /// Waits for a process to exit on the current thread
        /// </summary>
        /// <param name="pid">Process ID</param>
        private Task WaitForProcessToExitAsync(int pid, CancellationToken cancellation)
        {
            logger.LogInformation($"Waiting for pid {pid} to exit");

            var tcs = new TaskCompletionSource<int>();

            cancellation.Register(() =>
            {
                tcs.SetCanceled();
            });

            using (var process = Process.GetProcesses().FirstOrDefault(p => p.Id == pid))
            {
                if (process.HasExited) tcs.SetResult(process.ExitCode);

                process.EnableRaisingEvents = true;

                process.Exited += (sender, args) =>
                {
                    tcs.SetResult(process.ExitCode);
                };

                process?.CloseMainWindow();
                process?.WaitForExit(1);
            }

            return tcs.Task;
        }


        internal bool RestartApp(bool update = false, bool waitForPid = true, bool asAdmin = false)
        {
            logger.LogDebug($"Restarting app: [update={update}; waitForPid={waitForPid}; asAdmin={asAdmin}]");

            List<string> args = new List<string>(Environment.GetCommandLineArgs());

            for (int i = 0; i < args.Count; i++)
            {
                if (!update && args[i] == options.CommandLineArgumentPrefix + options.UpdateArgumentName)
                {
                    args[i] = string.Empty;
                }
                else if (args[i] == options.CommandLineArgumentPrefix + options.WaitArgumentName)
                {
                    args[i] = string.Empty;
                    if (i + 1 < args.Count)
                        args[++i] = string.Empty;
                }
            }

            if (waitForPid && !args.Contains(options.CommandLineArgumentPrefix + options.WaitArgumentName))
            {
                args.Add(options.CommandLineArgumentPrefix + options.WaitArgumentName);
                args.Add(Process.GetCurrentProcess().Id.ToString());
            }

            if (update && !args.Contains(options.CommandLineArgumentPrefix + options.UpdateArgumentName))
                args.Add(options.CommandLineArgumentPrefix + options.UpdateArgumentName);

            string arguments = args.NotEmpty().Distinct().AppendAll(" ");

            ProcessStartInfo startInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, arguments)
            {
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = true,
                Verb = (asAdmin) ? "runas" : string.Empty
            };

            try
            {
                Process proc = Process.Start(startInfo);

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to restart the application");

                //HandleException(owner, e);

                return false;
            }

            // we will never reach this part of the code
            return true;
        }

        public void Dispose()
        {

        }
    }
}

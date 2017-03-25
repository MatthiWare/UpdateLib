﻿using System;
using System.Text;
using MatthiWare.UpdateLib.Files;
using System.ComponentModel;
using System.Windows.Forms;
using MatthiWare.UpdateLib.UI;
using System.Drawing;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Logging.Writers;
using MatthiWare.UpdateLib.Logging;

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
                {
                    lock (synclock)
                    {
                        instance = new Updater();
                    }
                }
                return instance;
            }
        }
        #endregion

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

        public bool ShowUpdateMessage { get; set; } = true;
        public bool ShowMessageOnNoUpdate { get; set; } = true;
        public bool ShowErrorMessage { get; set; } = true;
        public PathVariableConverter Converter { get; private set; }

        public CleanUpTask CleanUpTask { get; private set; }
        public UpdateCacheTask UpdateCacheTask { get; private set; }

        private bool initialized = false;

        internal string RemoteBasePath { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Updater"/> with the default settings. 
        /// </summary>
        private Updater()
        {
            Converter = new PathVariableConverter();

            //Logger.Writers.Add(new ConsoleLogWriter());
            //Logger.Writers.Add(new FileLogWriter());
        }

        public void Initialize()
        {
            CleanUpTask = new CleanUpTask(".");
            CleanUpTask.Start();

            UpdateCacheTask = new UpdateCacheTask();
            UpdateCacheTask.Start();

            initialized = true;
        }

        /// <summary>
        /// Starting the update process
        /// </summary>
        public void CheckForUpdates()
        {
            if (!initialized)
                throw new InvalidOperationException("The updater needs to be initialized first.");

            if (string.IsNullOrEmpty(UpdateURL))
                throw new ArgumentException("You need to specifify a update url", "UpdateURL");

            CheckForUpdatesTask task = new CheckForUpdatesTask(UpdateURL);
            task.TaskCompleted += (o, e) => { CheckForUpdatesCompleted?.Invoke(task, new CheckForUpdatesCompletedEventArgs(task.Version, task.Result, e)); };
            task.Start();
        }

        /// <summary>
        /// Start the update process asynchronously
        /// </summary>
        public void CheckForUpdatesAsync()
        {

        }

        public HashCacheFile GetCache()
        {
            return UpdateCacheTask.AwaitTask();
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

    }
}

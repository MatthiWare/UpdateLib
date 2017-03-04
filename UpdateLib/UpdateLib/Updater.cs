using System;
using System.Text;
using MatthiWare.UpdateLib.Files;
using System.ComponentModel;
using System.Windows.Forms;
using MatthiWare.UpdateLib.UI;
using System.Drawing;
using MatthiWare.UpdateLib.Tasks;

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
            ShowUpdateMessage = true;
            ShowMessageOnNoUpdate = false;
            Converter = new PathVariableConverter();
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

            //WebClient wc = new WebClient();
            //wc.DownloadFileCompleted += UpdateFile_DownloadCompleted;
            //wc.DownloadFileAsync(new Uri(UpdateURL), m_localUpdateFile);

            CheckForUpdatesTask task = new CheckForUpdatesTask(UpdateURL);
            task.TaskCompleted += Task_TaskCompleted;
            task.Start();

        }

        private void Task_TaskCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageDialog msgError = new MessageDialog(
                    "Error",
                    "Unable to get the update information",
                    "There has been a problem getting the needed update information \nPlease contact customer support!",
                    SystemIcons.Error, MessageBoxButtons.OK);

                msgError.ShowDialog();

                return;
            }

            if (e.Cancelled)
                return;
        }

        public HashCacheFile GetCache()
        {
            UpdateCacheTask.AwaitTask();
            return UpdateCacheTask.Result;
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

using System;
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
                    lock (synclock)
                        if (instance == null)
                            instance = new Updater();

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

        /// <summary>
        /// Gets the clean up task
        /// </summary>
        public CleanUpTask CleanUpTask { get; private set; }

        /// <summary>
        /// Gets the update cache task
        /// </summary>
        public UpdateCacheTask UpdateCacheTask { get; private set; }

        private bool initialized = false;

        internal string RemoteBasePath { get; set; }

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
            CleanUpTask = new CleanUpTask(".");
            CleanUpTask.ConfigureAwait(false).Start();

            UpdateCacheTask = new UpdateCacheTask();
            UpdateCacheTask.Start();

            initialized = true;
        }

        /// <summary>
        /// Starting the update process
        /// </summary>
        /// <returns>Whether or not there is an update available and the latest version</returns>
        public CheckForUpdatesTask.Data CheckForUpdates()
        {
            return CheckForUpdatesAsync().AwaitTask();
        }

        /// <summary>
        /// Starting the update process
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <returns>Whether or not there is an update available and the latest version</returns>
        public CheckForUpdatesTask.Data CheckForUpdates(IWin32Window owner)
        {
            return CheckForUpdatesAsync(owner).AwaitTask();
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
            if (!initialized)
                throw new InvalidOperationException("The updater needs to be initialized first.");

            if (string.IsNullOrEmpty(UpdateURL))
                throw new ArgumentException("You need to specifify an update url", nameof(UpdateURL));

            CheckForUpdatesTask task = new CheckForUpdatesTask(UpdateURL);
            task.TaskCompleted += (o, e) => 
            {
                if (!task.Result.UpdateAvailable) // no updates available
                    return;

                DialogResult result = MessageDialog.Show(
                        "Update available",
                        $"Version {task.Result.Version} available",
                        "Update now?\nPress yes to update or no to cancel.",
                        SystemIcons.Question);


                if (result == DialogResult.Yes)
                {
                    UpdaterForm updateForm = new UpdaterForm(task.Result.UpdateFile);
                    updateForm.ShowDialog(owner);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MatthiWare.UpdateLib.Files;
using System.Net;
using MatthiWare.UpdateLib.Encoders;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Windows.Forms;
using MatthiWare.UpdateLib.UI;
using System.Drawing;
using System.Threading;
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

        //private void UpdateFile_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    WebClient download_client = (WebClient)sender;
        //    download_client.Dispose();

        //    // the update process got cancelled. 
        //    if (e.Cancelled)
        //        return;

        //    // error reporting
        //    if (e.Error != null)
        //    {
        //        Debug.WriteLine(String.Concat(e.Error.Message, "\n", e.Error.StackTrace));

        //        if (ShowErrorMessage)
        //            new MessageDialog(
        //                "Error", 
        //                "Unable to get the update information", 
        //                "There has been a problem getting the needed update information  \nPlease contact customer support!", 
        //                SystemIcons.Error, MessageBoxButtons.OK).ShowDialog();

        //        return;
        //    }

        //    Action caller = new Action(CheckForUpdatesTask);
        //    caller.BeginInvoke(new AsyncCallback(r => caller.EndInvoke(r)), null);
        //}

        //private void CheckForUpdatesTask()
        //{
        //    UpdateFile updateFile = LoadUpdateFile();

        //    HashCacheFile cache = GetCache();
        //    CleanUpTask.AwaitTask();

        //    CheckForUpdatedFilesTask checkForUpdatesTask = new CheckForUpdatedFilesTask(updateFile, cache, Converter);
        //    checkForUpdatesTask.Start();
        //    checkForUpdatesTask.AwaitTask();

        //    Console.WriteLine("[INFO]: CheckForUpdatesTask: {0}", (checkForUpdatesTask.Result) ? "New version available!" : "Latest version!");

        //    if (!checkForUpdatesTask.Result)
        //        return;

        //    DialogResult result = DialogResult.Yes;
        //    if (ShowUpdateMessage)
        //        result = new MessageDialog(
        //            "Update available",
        //            String.Format("Version {0} available", updateFile.VersionString),
        //            "Update now?\nPress yes to update or no to cancel.",
        //            SystemIcons.Question).ShowDialog();

        //    if (result != DialogResult.Yes)
        //        return;

        //    // start actual updateform
        //    UpdaterForm updateForm = new UpdaterForm(updateFile);
        //    updateForm.ShowDialog();
        //}

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

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

        public string UpdateURL { get; set; }
        private string m_localUpdateFile;

        public bool ShowUpdateMessage { get; set; }
        public bool ShowMessageOnNoUpdate { get; set; }

        public PathVariableConverter Converter { get; private set; }

        private CleanUpTask cleanUpTask;

        /// <summary>
        /// Initializes a new instance of <see cref="Updater"/> with the default settings. 
        /// </summary>
        private Updater()
        {
            ShowUpdateMessage = true;
            ShowMessageOnNoUpdate = false;
            Converter = new PathVariableConverter();
        }


        /// <summary>
        /// Starting the update process
        /// </summary>
        public void CheckForUpdates()
        {
            cleanUpTask = new CleanUpTask(".");
            cleanUpTask.Start();

            if (String.IsNullOrEmpty(UpdateURL))
                throw new ArgumentException("You need to specifify a update url", "UpdateURL");

            m_localUpdateFile = String.Concat("./", GetFileNameFromUrl(UpdateURL));

            WebClient wc = new WebClient();
            wc.DownloadFileCompleted += UpdateFile_DownloadCompleted;
            wc.DownloadFileAsync(new Uri(UpdateURL), m_localUpdateFile);
        }

        private void UpdateFile_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            WebClient download_client = (WebClient)sender;
            download_client.Dispose();

            // the update process got cancelled. 
            if (e.Cancelled)
                return;

            // error reporting
            if (e.Error != null)
            {
                Debug.WriteLine(String.Concat(e.Error.Message, "\n", e.Error.StackTrace));

                return;
            }

            UpdateFile updateFile = LoadUpdateFile();



            // check if there is a no new version
            //if (onlineVersion <= localVersion)
            //{
            //    if (ShowMessageOnNoUpdate)
            //        MessageBox.Show("You already have the latest version.", "Updater");

            //    return;
            //}

            DialogResult result = DialogResult.OK;
            if (ShowUpdateMessage)
                result = new MessageDialog(
                    "Update available",
                    String.Format("Version {0} available", updateFile.VersionString),
                    "Update now?\nPress yes to update or no to cancel.",
                    SystemIcons.Question).ShowDialog();

            if (result != DialogResult.OK)
                return;


            // Wait for clean up task to complete if needed.
            cleanUpTask.AwaitTask();


            // start actual updateform


        }

        private UpdateFile LoadUpdateFile()
        {
            return UpdateFile.Load(m_localUpdateFile);
        }

        private String GetFileNameFromUrl(String url)
        {
            String[] tokens = url.Split('/');
            return tokens[tokens.Length - 1];
        }

    }
}

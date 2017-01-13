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

namespace MatthiWare.UpdateLib
{
    public class Updater : Component
    {
        public string UpdateURL { get; set; }
        private string m_localUpdateFile;

        public bool ShowUpdateMessage { get; set; }
        public bool ShowMessageOnNoUpdate { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Updater"/> with the default settings. 
        /// </summary>
        public Updater()
        {
            ShowUpdateMessage = true;
            ShowMessageOnNoUpdate = false;
        }


        /// <summary>
        /// Starting the update process
        /// </summary>
        public void CheckForUpdates()
        {
            CleanUp();

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

            UpdateInfoFile updateFile = LoadUpdateFile();

            Version localVersion = GetCurrentVersion();
            Version onlineVersion = new Version(updateFile.VersionString);

            // check if there is a no new version
            if (onlineVersion <= localVersion)
            {
                if (ShowMessageOnNoUpdate)
                    MessageBox.Show("You already have the latest version.", "Updater");

                return;
            }

            DialogResult result = DialogResult.OK;
            if (ShowUpdateMessage)
                result = new MessageDialog("Update available!", "")

            if (result != DialogResult.OK)
                return;

            // start actual updateform


        }

        private UpdateInfoFile LoadUpdateFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UpdateInfoFile));

            using (Stream s = File.Open(m_localUpdateFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                return (UpdateInfoFile)serializer.Deserialize(s);
            }
        }

        private String GetFileNameFromUrl(String url)
        {
            String[] tokens = url.Split('/');
            return tokens[tokens.Length - 1];
        }

        private Version GetCurrentVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        private void CleanUp()
        {

        }

    }
}

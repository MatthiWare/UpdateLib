using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Files;
using System.Net;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class UpdatePart : UserControl
    {

        public UpdateInfoFile UpdateFile { get; set; }

        private int amountToDownload;

        public UpdatePart(UpdateInfoFile uif)
        {
            InitializeComponent();

            UpdateFile = uif;

            FillListView();
        }

        private void FillListView()
        {
            amountToDownload = UpdateFile.Files.Count;

            foreach (UpdateFile file in UpdateFile.Files)
            {
                ListViewItem lvItem = new ListViewItem(new string[] { "", file.Name, "Ready to download", "0%" });
                lvItem.Tag = file;

                lvItems.Items.Add(lvItem);
            }
        }

        public void StartUpdate()
        {
            foreach (ListViewItem item in lvItems.Items)
            {
                Action<ListViewItem> downloadAction = new Action<ListViewItem>(StartDownloadItem);
                downloadAction.BeginInvoke(item, null, null);
                
            }
        }

        public void CancelUpdate()
        {

        }

        private void StartDownloadItem(ListViewItem item)
        {
            UpdateFile file = (UpdateFile)item.Tag;

            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;

        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
           
        }
    }
}

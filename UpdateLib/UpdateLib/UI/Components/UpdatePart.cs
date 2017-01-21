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
using System.Threading;

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



        }

        Random rnd = new Random();
        private void Test(ListViewItem item)
        {

            int wait = rnd.Next(2000);

            Thread.Sleep(wait);

            SetImageKey(item, "status_download");
            SetSubItemText(item.SubItems[2], "Downloading..");

            wait = rnd.Next(100);
            for (int i = 0; i <= 100; i++)
            {
                SetSubItemText(item.SubItems[3], String.Format("{0}%", i));
                Thread.Sleep(wait);
            }

            bool val = rnd.Next(0, 2) == 0 ? false : true;
            SetSubItemText(item.SubItems[2], val ? "Done" : "Error");

            SetImageKey(item, val ? "status_done" : "status_error");
            

            int amountLeft = Interlocked.Decrement(ref amountToDownload);

            if (amountLeft != 0)
                return;
            

        }


        private delegate void SetImageKeyInvoker(ListViewItem item, string key);
        private void SetImageKey(ListViewItem item, string key)
        {
            if (InvokeRequired)
            {
                Invoke(new SetImageKeyInvoker(SetImageKey), item, key);
                return;
            }
            item.ImageKey = key;
        }

        private delegate void SetSubItemTextInvoker(ListViewItem.ListViewSubItem item, string key);
        private void SetSubItemText(ListViewItem.ListViewSubItem item, string key)
        {
            if (InvokeRequired)
            {
                Invoke(new SetSubItemTextInvoker(SetSubItemText), item, key);
                return;
            }

            item.Text = key;
        }
    }
}

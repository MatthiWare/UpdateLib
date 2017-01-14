using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public partial class UpdaterForm : Form
    {
        private UpdateInfoFile updateInfoFile;

        private int amountOfDownloadsToGo;
        private bool errorOccured = false;

        public UpdaterForm()
        {
            InitializeComponent();           
        }

        public UpdaterForm(UpdateInfoFile updateFile)
           : this()
        {
            updateInfoFile = updateFile;
            amountOfDownloadsToGo = updateFile.Files.Count;
            FillList();
        }

        private void FillList()
        {
            foreach (UpdateFile file in updateInfoFile.Files)
            {
                string[] data = new string[] { "", file.Name, "Ready to download", "0%"};
                ListViewItem lvItem = new ListViewItem(data);

                lvItems.Items.Add(lvItem);
            }
        }

        private void btnUpdateCancel_Click(object sender, EventArgs e)
        {
            List<WaitHandle> waithandles = new List<WaitHandle>(lvItems.Items.Count);

            btnUpdateCancel.Enabled = false;

            foreach(ListViewItem lvItem in lvItems.Items)
            {
                Action<ListViewItem> a = new Action<ListViewItem>(Test);
                waithandles.Add(a.BeginInvoke(lvItem, null, null).AsyncWaitHandle);
                
            }

            btnUpdateCancel.Enabled = true;
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
                SetSubItemText(item.SubItems[3], String.Format("{0}%", i ));
                Thread.Sleep(wait);
            }

            bool val = rnd.Next(0, 2) == 0 ? false : true;
            SetSubItemText(item.SubItems[2], val? "Done":"Error");

            SetImageKey(item, val ? "status_done" : "status_error");

            if (!val)
                errorOccured = true;

            int amountLeft = Interlocked.Decrement(ref amountOfDownloadsToGo);

            if (amountLeft != 0)
                return;

            if (errorOccured)
            {
                SetErrored();
                return;
            }

            SetDone();
                
        }

        private void SetErrored()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(SetErrored));
                return;
            }

            btnUpdateCancel.Enabled = true;
            btnUpdateCancel.Text = "Error";

        }

        private void SetDone()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(SetDone));
                return;
            }

            btnUpdateCancel.Enabled = true;
            btnUpdateCancel.Text = "Finish";
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
                Invoke(new SetSubItemTextInvoker(SetSubItemText),item, key);
                return;
            }

            item.Text = key;
        }

        private void lblUpdateLibLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MatthiWare/UpdateLib");
        }
    }
}

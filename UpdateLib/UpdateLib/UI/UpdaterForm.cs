using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public UpdaterForm()
        {
            InitializeComponent();

            lvItems.SmallImageList.Images.Add("status_done", Resources.status_done);
            lvItems.SmallImageList.Images.Add("status_download", Resources.status_download);
            lvItems.SmallImageList.Images.Add("status_error", Resources.status_error);

            
        }

        public UpdaterForm(UpdateInfoFile updateFile)
           : this()
        {
            updateInfoFile = updateFile;
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
            foreach(ListViewItem lvItem in lvItems.Items)
            {
                Action<ListViewItem> a = new Action<ListViewItem>(Test);
                a.BeginInvoke(lvItem, null, null);
            }
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
    }
}

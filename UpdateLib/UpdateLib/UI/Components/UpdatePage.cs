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
using MatthiWare.UpdateLib.Properties;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class UpdatePage : UserControl, IWizardPage
    {

        public UpdateFile UpdateFile { get; set; }

        public event EventHandler PageUpdate;

        private int amountToDownload;

        public UpdatePage(UpdaterForm parent)
        {
            InitializeComponent();

            _updaterForm = parent;

            UpdateFile = parent.updateInfoFile;

            ImageList ilItems = MakeImageList();
            lvItems.SmallImageList = ilItems;

            FillListView();
        }

        private ImageList MakeImageList()
        {
            ImageList imgList = new ImageList();

            imgList.Images.Add("status_download", Resources.status_download);
            imgList.Images.Add("status_done", Resources.status_done);
            imgList.Images.Add("status_error", Resources.status_error);
            imgList.Images.Add("status_info", Resources.status_info);
            imgList.Images.Add("status_update", Resources.status_update);

            return imgList;
        }

        private void FillListView()
        {
            amountToDownload = UpdateFile.Count;

            AddDirectoryToListView(UpdateFile.ApplicationDirectory);
            AddDirectoryToListView(UpdateFile.OtherDirectory);
        }

        private void AddDirectoryToListView(DirectoryEntry dir)
        {
            foreach(FileEntry file in dir.Files)
            {
                ListViewItem lvItem = new ListViewItem(new string[] { "", file.Name, "Ready to download", "0%" });
                lvItem.Tag = file;

                lvItems.Items.Add(lvItem);
            }

            foreach (DirectoryEntry subDir in dir.Directories)
                AddDirectoryToListView(subDir);
        }

        public void StartUpdate()
        {
            IsBusy = true;
            PageUpdate?.Invoke(this, new EventArgs());
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

            Test(item);

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

            IsBusy = false;
            IsDone = true;

            PageUpdate?.Invoke(this, new EventArgs());
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

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            StartUpdate();
        }

        private UpdaterForm _updaterForm;
        public UpdaterForm UpdaterForm
        {
            get
            {
                return _updaterForm;
            }
        }

        public UserControl Conent
        {
            get
            {
                return this;
            }
        }

        public bool NeedsCancel
        {
            get
            {
                return true;
            }
        }

        public bool NeedsExecution
        {
            get
            {
                return true;
            }
        }

        private bool _isbusy;
        public bool IsBusy
        {
            get
            {
                return _isbusy;
            }

            set
            {
                _isbusy = value;
            }
        }

        public void PageEntered()
        {

        }

        private bool _isdone;
        public bool IsDone
        {
            get
            {
                return _isdone;
            }

            set
            {
                _isdone = value;
            }
        }

        public string Title
        {
            get
            {
                return lblHeader.Text;
            }
        }
    }
}

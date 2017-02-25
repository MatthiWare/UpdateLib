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
using System.Diagnostics;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Security;
using System.Security.Cryptography;

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
            imgList.Images.Add("status_warning", Resources.status_warning);

            return imgList;
        }

        private void FillListView()
        {
            amountToDownload = UpdateFile.Count;

            lvItems.BeginUpdate();

            AddDirectoryToListView(UpdateFile.ApplicationDirectory);
            AddDirectoryToListView(UpdateFile.OtherDirectory);

            lvItems.Columns[5].Width = -1;
            lvItems.Columns[1].Width = -1;

            lvItems.EndUpdate();
        }

        private void AddDirectoryToListView(DirectoryEntry dir)
        {
            foreach(FileEntry file in dir.Files)
            {
                
                ListViewItem lvItem = new ListViewItem(new string[] { "", file.Name, "Ready to download", "0%", file.Description, Updater.Instance.Converter.Replace(file.DestinationLocation) });
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
                Console.WriteLine("UpdatePage thread start: {0}", Thread.CurrentThread.ManagedThreadId);
                DownloadTask task = new DownloadTask(item);
                task.TaskProgressChanged += Task_TaskProgressChanged;
                task.TaskCompleted += Task_TaskCompleted;
                task.Start();

                SetImageKey(item, "status_download");
                SetSubItemText(item.SubItems[2], "Downloading..");
            }
        }

        private void Task_TaskCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadTask task = (DownloadTask)sender;
            Console.WriteLine("UpdatePage thread completed: {0}", Thread.CurrentThread.ManagedThreadId);

            int amountLeft = Interlocked.Decrement(ref amountToDownload);

            if (amountLeft == 0)
            {
                IsBusy = false;
                IsDone = true;
                PageUpdate?.Invoke(this, new EventArgs());
            }

            if (e.Cancelled)
            {
                Console.WriteLine("[INFO][DownloadTask]: Cancelled -> '{0}' ", task.Entry.Name);

                SetSubItemText(task.Item.SubItems[2],  "Cancelled");

                SetImageKey(task.Item,"status_warning");

                return;
            }

            if (e.Error != null)
            {
                Console.WriteLine("[ERROR][DownloadTask]: {0}\n{1}", e.Error.Message, e.Error.StackTrace);

                SetSubItemText(task.Item.SubItems[2], "Error");

                SetImageKey(task.Item, "status_error");

                return;
            }

            // Everything went good lets just check the MD5 hash again to be a bit more secure against attacks
            if (!VerifyDownloadedFileSignature(task))
            {
                Console.WriteLine("[ERROR][DownloadTask]: Signature match fail for file: {0}", task.Entry.Name);

                SetSubItemText(task.Item.SubItems[2], "Error");

                SetImageKey(task.Item, "status_error");

                return;
            }


            SetSubItemText(task.Item.SubItems[2], "Done");

            SetImageKey(task.Item, "status_done");
        }

        private bool VerifyDownloadedFileSignature(DownloadTask task)
        {
            string localFile = Updater.Instance.Converter.Replace(task.Entry.DestinationLocation);
            string md5local = HashUtil.GetHash(localFile);

            return md5local.Equals(task.Entry.Hash);
        }

        private void Task_TaskProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadTask task = (DownloadTask)sender;

            SetSubItemText(task.Item.SubItems[3], string.Format("{0}%", e.ProgressPercentage));
        }

        public void CancelUpdate()
        {

        }

        private void StartDownloadItem(ListViewItem item)
        {
            
            //Test(item);

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
        
        private void SetImageKey(ListViewItem item, string key)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ListViewItem, string>(SetImageKey), item, key);
                return;
            }
            item.ImageKey = key;
        }
        
        private void SetSubItemText(ListViewItem.ListViewSubItem item, string key)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ListViewItem.ListViewSubItem, string>(SetSubItemText), item, key);
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

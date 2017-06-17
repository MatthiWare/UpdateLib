using System;
using System.ComponentModel;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Files;
using System.Threading;
using MatthiWare.UpdateLib.Properties;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Security;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Threading;
using System.Collections.Generic;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class UpdatePage : UserControl, IWizardPage
    {

        public UpdateFile UpdateFile { get; set; }

        public event EventHandler PageUpdate;

        private AtomicInteger amountToDownload = new AtomicInteger();

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
            amountToDownload.Value = UpdateFile.FileCount;

            lvItems.BeginUpdate();

            foreach (DirectoryEntry dir in UpdateFile.Folders)
                AddDirectoryToListView(dir);

            lvItems.Columns[5].Width = -1;
            lvItems.Columns[1].Width = -1;

            lvItems.EndUpdate();
        }

        private void AddDirectoryToListView(DirectoryEntry dir)
        {
            foreach (FileEntry file in dir.Files)
            {

                ListViewItem item = new ListViewItem(new string[] { string.Empty, file.Name, "Ready to download", "0%", file.Description, Updater.Instance.Converter.Replace(file.DestinationLocation) });
                item.Tag = file;

                DownloadTask task = new DownloadTask(item);
                task.TaskProgressChanged += Task_TaskProgressChanged;
                task.TaskCompleted += Task_TaskCompleted;

                downloadTasks.Add(task);

                lvItems.Items.Add(item);
            }

            foreach (DirectoryEntry subDir in dir.Directories)
                AddDirectoryToListView(subDir);
        }

        private List<DownloadTask> downloadTasks = new List<DownloadTask>();

        public void StartUpdate()
        {
            IsBusy = true;
            PageUpdate?.Invoke(this, new EventArgs());

            foreach (ListViewItem item in lvItems.Items)
            {
                SetImageKey(item, "status_download");
                SetSubItemText(item.SubItems[2], "Downloading..");
            }

            foreach (DownloadTask task in downloadTasks)
                task.Start();
        }

        private void Task_TaskCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadTask task = (DownloadTask)sender;

            if (amountToDownload.Decrement() == 0)
            {
                IsBusy = false;
                IsDone = true;
                PageUpdate?.Invoke(this, EventArgs.Empty);
            }

            if (e.Cancelled)
            {
                Updater.Instance.Logger.Info(nameof(UpdatePage), nameof(StartUpdate), $"Rolled back -> '{task.Entry.Name}'");

                SetSubItemText(task.Item.SubItems[2], "Rolled back");

                SetImageKey(task.Item, "status_warning");

                return;
            }

            if (e.Error != null)
            {
                HasErrors = true;
                PageUpdate?.Invoke(this, EventArgs.Empty);

                Updater.Instance.Logger.Error(nameof(UpdatePage), nameof(StartUpdate), e.Error);

                SetSubItemText(task.Item.SubItems[2], "Error");

                SetImageKey(task.Item, "status_error");

                return;
            }

            SetSubItemText(task.Item.SubItems[2], "Done");

            SetImageKey(task.Item, "status_done");
        }

        private void Task_TaskProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadTask task = (DownloadTask)sender;

            SetSubItemText(task.Item.SubItems[3], $"{e.ProgressPercentage}%");
        }

        public void CancelUpdate()
        {

        }

        private void StartDownloadItem(ListViewItem item)
        {

            //Test(item);

        }

        private void SetImageKey(ListViewItem item, string key)
        {
            this.InvokeOnUI(() => item.ImageKey = key);
            //if (InvokeRequired)
            //{
            //    Invoke(new Action<ListViewItem, string>(SetImageKey), item, key);
            //    return;
            //}
            //item.ImageKey = key;
        }

        private void SetSubItemText(ListViewItem.ListViewSubItem item, string key)
        {
            this.InvokeOnUI(() => item.Text = key);
            //if (InvokeRequired)
            //{
            //    Invoke(new Action<ListViewItem.ListViewSubItem, string>(SetSubItemText), item, key);
            //    return;
            //}

            //item.Text = key;
        }

        public void Cancel()
        {
            if (HasErrors && NeedsRollBack)
                Rollback();

            IsDone = true;


            PageUpdate?.Invoke(this, EventArgs.Empty);
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

        public bool NeedsRollBack { get { return true; } }
        
        public bool IsBusy
        {
            get;set;
            
        }

        public void PageEntered()
        {

        }

        public void UpdateState()
        {

        }

        public void Rollback()
        {
            IsBusy = true;

            foreach (DownloadTask task in downloadTasks)
            {
                if (!task.IsCancelled)
                    task.Cancel();

                SetSubItemText(task.Item.SubItems[2], "Rolled back");

                SetImageKey(task.Item, "status_warning");
            }

            IsBusy = false;
            HasErrors = false;

            PageUpdate?.Invoke(this, EventArgs.Empty);
        }

        public bool IsDone
        {
            get; set;
        }

        public string Title
        {
            get
            {
                return lblHeader.Text;
            }
        }

        public bool HasErrors
        {
            get; set;
        }
    }
}

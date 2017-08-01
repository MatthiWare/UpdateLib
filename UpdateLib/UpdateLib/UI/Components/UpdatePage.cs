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
using System.Linq;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.UI.Components
{
    public partial class UpdatePage : UserControl, IWizardPage
    {

        public UpdateFile UpdateFile { get; set; }

        public event EventHandler PageUpdate;

        private AtomicInteger amountToDownload = new AtomicInteger();
        private List<UpdatableTask> m_updateTasks = new List<UpdatableTask>();
        private bool hasRegTask = false;

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

            AddDirectoryToListView(
                UpdateFile.Folders
                .SelectMany(dir => dir.GetItems())
                .Select(e => e as FileEntry)
                .Distinct()
                .NotNull());

            AddRegistryToListView(UpdateFile.Registry
                .SelectMany(dir => dir.GetItems())
                .Select(e => e as RegistryKeyEntry)
                .Distinct()
                .NotNull());

            lvItems.Columns[4].Width = -1;
            lvItems.Columns[0].Width = -1;

            lvItems.EndUpdate();
        }

        private void AddDirectoryToListView(IEnumerable<FileEntry> files)
        {
            foreach (FileEntry entry in files)
            {

                ListViewItem item = new ListViewItem(new string[] { entry.Name, "Ready to download", "0%", entry.Description, Updater.Instance.Converter.Replace(entry.DestinationLocation) });
                item.Tag = entry;

                DownloadTask task = new DownloadTask(item, entry);
                task.TaskProgressChanged += Task_TaskProgressChanged;
                task.TaskCompleted += Task_TaskCompleted;

                m_updateTasks.Add(task);

                lvItems.Items.Add(item);
            }
        }

        private void AddRegistryToListView(IEnumerable<RegistryKeyEntry> keys)
        {
            if (keys.Count() == 0)
                return;

            amountToDownload.Increment();
            hasRegTask = true;

            ListViewItem item = new ListViewItem(new string[] { "Update registry", "Waiting for other tasks to complete", "0%", "Applies changes to the registry" });

            UpdateRegistryTask task = new UpdateRegistryTask(item, keys);
            task.TaskProgressChanged += Task_TaskProgressChanged;
            task.TaskCompleted += Task_TaskCompleted;

            m_updateTasks.Add(task);

            lvItems.Items.Add(item);
        }

        public void StartUpdate()
        {
            IsBusy = true;
            PageUpdate?.Invoke(this, new EventArgs());

            IEnumerable<DownloadTask> downloadTasks = m_updateTasks.Select(x => x as DownloadTask).NotNull();

            foreach (DownloadTask task in downloadTasks)
            {
                SetImageKey(task.Item, "status_download");
                SetSubItemText(task.Item.SubItems[1], "Downloading..");

                task.Start();
            }


            if (hasRegTask && downloadTasks.Count() == 0)
                StartRegUpdate();

        }

        private void StartRegUpdate()
        {
            UpdateRegistryTask task = m_updateTasks.Select(x => x as UpdateRegistryTask).NotNull().FirstOrDefault();

            if (task == null)
                return;

            SetImageKey(task.Item, "status_download");
            SetSubItemText(task.Item.SubItems[1], "Updating..");

            task.Start();

        }

        private void Task_TaskCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdatableTask task = (UpdatableTask)sender;

            if (e.Cancelled)
            {
                SetSubItemText(task.Item.SubItems[1], "Rolled back");
                SetImageKey(task.Item, "status_warning");

                return;
            }

            if (e.Error != null)
            {
                HasErrors = true;
                PageUpdate?.Invoke(this, EventArgs.Empty);

                Updater.Instance.Logger.Error(nameof(UpdatePage), nameof(StartUpdate), e.Error);

                SetSubItemText(task.Item.SubItems[1], "Error");
                SetImageKey(task.Item, "status_error");

                return;
            }

            SetSubItemText(task.Item.SubItems[1], "Done");
            SetImageKey(task.Item, "status_done");

            int left = amountToDownload.Decrement();

            if (left == 1 && hasRegTask)
                StartRegUpdate();
            else if (left == 0)
            {
                IsBusy = false;
                IsDone = true;
                PageUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Task_TaskProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdatableTask task = (UpdatableTask)sender;

            SetSubItemText(task.Item.SubItems[2], $"{e.ProgressPercentage}%");
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
            if (NeedsRollBack)
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
            get; set;

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

            foreach (UpdatableTask task in m_updateTasks)
            {
                if (task.IsCancelled || (!task.IsCompleted && !task.IsRunning))
                {
                    SetSubItemText(task.Item.SubItems[1], "No action");

                    SetImageKey(task.Item, "status_warning");

                    continue;
                }


                task.Cancel();

                SetSubItemText(task.Item.SubItems[1], "Rolled back");

                SetImageKey(task.Item, "status_warning");
            }

            IsBusy = false;
            HasErrors = false;
            IsDone = true;

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

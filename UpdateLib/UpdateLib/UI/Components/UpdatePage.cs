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
        private Dictionary<UpdatableTask, ListViewItem> m_items = new Dictionary<UpdatableTask, ListViewItem>();
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
                ListViewItem item = new ListViewItem(new string[] { entry.Name, "Ready to download", "0%", entry.Description, Updater.Instance.Converter.Convert(entry.DestinationLocation) });
                item.Tag = entry;

                DownloadTask task = new DownloadTask(entry);
                task.TaskProgressChanged += Task_TaskProgressChanged;
                task.TaskCompleted += Task_TaskCompleted;

                m_items.Add(task, item);

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

            UpdateRegistryTask task = new UpdateRegistryTask(keys);
            task.TaskProgressChanged += Task_TaskProgressChanged;
            task.TaskCompleted += Task_TaskCompleted;

            m_items.Add(task, item);

            lvItems.Items.Add(item);
        }

        public void StartUpdate()
        {
            IsBusy = true;
            PageUpdate?.Invoke(this, new EventArgs());

            var items = m_items.Where(x => (x.Key as DownloadTask != null));

            foreach (var kvp in items)
            {
                SetImageKey(kvp.Value, "status_download");
                SetSubItemText(kvp.Value.SubItems[1], "Downloading..");

                kvp.Key.Start();
            }


            if (hasRegTask && items.Count() == 0)
                StartRegUpdate();

        }

        private void StartRegUpdate()
        {
            var kvp = m_items.Where(x => (x.Key as UpdateRegistryTask) != null).NotNull().FirstOrDefault();

            if (kvp.Key == null || kvp.Value == null)
                return;

            var view = kvp.Value;

            SetImageKey(view, "status_download");
            SetSubItemText(view.SubItems[1], "Updating..");

            kvp.Key.Start();

        }

        private void Task_TaskCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var task = (UpdatableTask)sender;
            var view = m_items[task];

            if (e.Cancelled)
            {
                SetSubItemText(view.SubItems[1], "Rolled back");
                SetImageKey(view, "status_warning");

                return;
            }

            if (e.Error != null)
            {
                HasErrors = true;
                PageUpdate?.Invoke(this, EventArgs.Empty);

                Updater.Instance.Logger.Error(nameof(UpdatePage), nameof(StartUpdate), e.Error);

                SetSubItemText(view.SubItems[1], "Error");
                SetImageKey(view, "status_error");

                return;
            }

            SetSubItemText(view.SubItems[1], "Done");
            SetImageKey(view, "status_done");

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

            SetSubItemText(m_items[task].SubItems[2], $"{e.ProgressPercentage}%");
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

            foreach (var item in m_items)
            {
                UpdatableTask task = item.Key;
                ListViewItem view = item.Value;

                if (task.IsCancelled || (!task.IsCompleted && !task.IsRunning))
                {
                    SetSubItemText(view.SubItems[1], "No action");

                    SetImageKey(view, "status_warning");

                    continue;
                }


                task.Cancel();

                SetSubItemText(view.SubItems[1], "Rolled back");

                SetImageKey(view, "status_warning");
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

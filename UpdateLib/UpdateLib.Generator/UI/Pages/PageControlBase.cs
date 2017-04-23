using MatthiWare.UpdateLib.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public class PageControlBase : UserControl
    {
        internal TestForm TestForm { get; set; }

        public PageControlBase()
        {

        }

        public void ShowLoader()
        {
            LoaderControl.Show(TestForm?.ContentPanel);
        }

        public void HideLoader()
        {
            LoaderControl.Hide(TestForm?.ContentPanel);
        }

        public bool IsPageInitialized { get; private set; } = false;

        private AsyncTask taskInitialize;

        public AsyncTask InitializePage(EventHandler<AsyncCompletedEventArgs> callBack)
        {
            if (IsPageInitialized || (taskInitialize != null && taskInitialize.IsRunning))
                return taskInitialize;

            taskInitialize = AsyncTaskFactory.From(new Action(() => OnPageInitialize()), null);

            taskInitialize.TaskCompleted += (o, e) =>
            {
                IsPageInitialized = !e.Cancelled && e.Error == null;

                callBack?.Invoke(this, e);
            };

            return taskInitialize.Start();
        }

        protected virtual void OnPageInitialize() { }

    }
}

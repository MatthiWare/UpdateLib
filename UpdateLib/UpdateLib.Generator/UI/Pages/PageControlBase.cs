using MatthiWare.UpdateLib.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public  abstract class PageControlBase : UserControl
    {
        public bool IsPageInitialized { get; private set; } = false;

        public AsyncTask InitializePage(EventHandler<AsyncCompletedEventArgs> callBack)
        {
            AsyncTask task = AsyncTaskFactory.From(new Action(() => OnPageInitialize()), null);

            task.TaskCompleted += (o, e) =>
            {
                IsPageInitialized = !e.Cancelled && e.Error == null;

                callBack?.Invoke(this, e);
            };

            return task.Start();
        }

        protected abstract void OnPageInitialize();

    }
}

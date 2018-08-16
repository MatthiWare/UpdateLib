/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Tasks;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public class PageControlBase : UserControl
    {
        internal MainForm TestForm { get; set; }

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

            taskInitialize = AsyncTaskFactory.From(new Action(OnPageInitialize), null);

            taskInitialize.TaskCompleted += (o, e) =>
            {
                IsPageInitialized = !e.Cancelled && e.Error == null;

                callBack?.Invoke(this, e);
            };

            return taskInitialize.Start();
        }

        protected virtual void OnPageInitialize() { }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PageControlBase
            // 
            this.DoubleBuffered = true;
            this.Name = "PageControlBase";
            this.ResumeLayout(false);

        }
    }
}

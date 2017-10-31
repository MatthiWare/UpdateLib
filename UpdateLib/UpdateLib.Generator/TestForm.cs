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

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Generator.UI;
using MatthiWare.UpdateLib.Generator.UI.Pages;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;

namespace MatthiWare.UpdateLib.Generator
{
    public partial class TestForm : Form
    {
        private Dictionary<string, PageControlBase> pageCache;
        private AsyncTask loadTask;
        private bool shouldShowNewPage = false;

        public TestForm()
        {
            InitializeComponent();

            pageCache = new Dictionary<string, PageControlBase>();

            LoadPagesTask().Start();
        }

        public bool TryGetPage(string key, out PageControlBase page)
        {
            return pageCache.TryGetValue(key, out page);
        }

        private AsyncTask LoadPagesTask()
        {
            if (loadTask == null)
            {
                LoaderControl.Show(ContentPanel);

                Action loadAction = new Action(() =>
                {
                    var pageType = typeof(PageControlBase);
                    var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(asm => asm.GetTypes())
                        .Where(type => pageType.IsAssignableFrom(type) && !type.IsAbstract && type.IsClass && pageType != type);

                    foreach (Type type in types)
                    {
                        var name = type.Name;

                        PageControlBase page = Activator.CreateInstance(type) as PageControlBase;
                        page.TestForm = this;

                        pageCache.Add(name, page);
                    }
                });

                loadTask = AsyncTaskFactory.From(loadAction, null);

                loadTask.TaskCompleted += (o, e) =>
                {
                    LoaderControl.Hide(ContentPanel);

                    btnTabInformation.PerformClick();
                };
            }

            return loadTask;
        }

        private void TestForm_Click(object sender, EventArgs e)
        {
            WindowState = (WindowState == FormWindowState.Maximized) ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void pbMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void pbMaximize_Click(object sender, EventArgs e)
        {
            MaximumSize = Screen.FromControl(this).WorkingArea.Size;
            WindowState = (WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal);
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void flatButton1_Click(object sender, EventArgs e)
        {
            LoadPage(nameof(InformationPage));
        }

        private void flatButton2_Click(object sender, EventArgs e)
        {
            LoadPage(nameof(FilesPage));
        }

        private bool LoadPage(string pageName)
        {
            loadTask.AwaitTask();

            PageControlBase page = null;
            bool success = TryGetPage(pageName, out page);

            if (success)
            {
                shouldShowNewPage = true;

                if (page.IsPageInitialized)
                {
                    AddControlToContentPanel(page);
                }
                else
                {
                    AddControlToContentPanel(null);

                    LoaderControl.Show(ContentPanel);

                    page.InitializePage((o, e) =>
                    {
                        LoaderControl.Hide(ContentPanel);

                        if (e.Cancelled)
                        {
                            ShowMessageBox(
                                "Page Load",
                                "Task cancelled",
                                "The loading of the page got cancelled.",
                                SystemIcons.Warning,
                                MessageBoxButtons.OK);

                            return;
                        }

                        if (e.Error != null)
                        {
                            ShowMessageBox(
                                "Page Load",
                                "Error occured when loading the page",
                                "Check the log files for more information!",
                                SystemIcons.Error,
                                MessageBoxButtons.OK);

                            return;
                        }

                        AddControlToContentPanel(page);
                    });
                }
            }

            return success;
        }

        private void ShowMessageBox(string title, string header, string desc, Icon icon, MessageBoxButtons buttons = MessageBoxButtons.YesNo)
        {
            MessageDialog.Show(
                this,
                title,
                header,
                desc,
                icon,
                buttons);
        }

        private void AddControlToContentPanel(Control toAdd)
        {
            if (!shouldShowNewPage)
                return;

            ContentPanel.SuspendLayout();

            ContentPanel.Controls.Clear();

            if (toAdd != null)
            {
                toAdd.Dock = DockStyle.Fill;
                ContentPanel.Controls.Add(toAdd);

                shouldShowNewPage = false;
            }

            ContentPanel.ResumeLayout();

        }

        private void btnTabBuild_Click(object sender, EventArgs e)
        {
            LoadPage(nameof(BuilderPage));
        }

        private void btnTabRegistry_Click(object sender, EventArgs e)
        {
            LoadPage(nameof(RegistryPage));
        }
    }
}

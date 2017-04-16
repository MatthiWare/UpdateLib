using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Generator.UI;
using MatthiWare.UpdateLib.Generator.UI.Pages;
using MatthiWare.UpdateLib.Tasks;
using System.Threading;

namespace MatthiWare.UpdateLib.Generator
{
    public partial class TestForm : Form
    {
        private Dictionary<string, PageControlBase> pageCache;
        private AsyncTask loadTask; 

        public TestForm()
        {
            InitializeComponent();

            pageCache = new Dictionary<string, PageControlBase>();

            LoadPagesTask().Start();
        }

        private AsyncTask LoadPagesTask()
        {
            if (loadTask == null)
            {
                LoaderControl.Show(ContentPanel);

                Action loadAction = new Action(()=> 
                {
                    var pageType = typeof(PageControlBase);
                    var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(asm => asm.GetTypes())
                        .Where(type => pageType.IsAssignableFrom(type) && !type.IsAbstract && type.IsClass && pageType != type);

                    foreach (Type type in types)
                    {
                        var name = type.Name;
                        PageControlBase page = Activator.CreateInstance(type) as PageControlBase;
                        //Thread.Sleep(5000);
                        pageCache.Add(name, page);
                    }
                });

                loadTask = AsyncTaskFactory.From(loadAction, null);

                loadTask.TaskCompleted += (o, e) => 
                {
                    LoaderControl.Hide(ContentPanel);
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void flatButton1_Click(object sender, EventArgs e)
        {
            LoadPage(nameof(InformationPage));
        }

        private void flatButton2_Click(object sender, EventArgs e)
        {
            LoaderControl.Hide(ContentPanel);
        }

        private bool LoadPage(string pageName)
        {
            PageControlBase control = null;
            bool success = pageCache.TryGetValue(pageName, out control);

            if (control != null)
            {
                ContentPanel.SuspendLayout();

                ContentPanel.Controls.Clear();
                ContentPanel.Controls.Add(control);

                control.Dock = DockStyle.Fill;

                ContentPanel.ResumeLayout();
            }

            return success;
        }

        private void btnTabBuild_Click(object sender, EventArgs e)
        {
            LoadPage(nameof(BuilderPage));
        }
    }
}

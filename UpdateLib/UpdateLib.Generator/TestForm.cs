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

namespace MatthiWare.UpdateLib.Generator
{
    public partial class TestForm : Form
    {
        private Dictionary<string, UserControl> pageCache;

        public TestForm()
        {
            InitializeComponent();

            pageCache = new Dictionary<string, UserControl>();
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
            var name = nameof(InformationPage);

            if (!LoadPage(name))
            {
                pageCache.Add(name, new InformationPage());
                LoadPage(name);
            }

        }

        private void flatButton2_Click(object sender, EventArgs e)
        {
            LoaderControl.Hide(ContentPanel);
        }

        private bool LoadPage(string pageName)
        {
            UserControl control = null;
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

        }
    }
}

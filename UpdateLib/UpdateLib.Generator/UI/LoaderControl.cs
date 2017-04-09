using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateLib.Generator.UI
{

    public partial class LoaderControl : UserControl
    {
        private static Dictionary<Control, LoaderControl> loaders = new Dictionary<Control, LoaderControl>();

        public LoaderControl()
        {
            InitializeComponent();
        }


        public static void Show(Control parent)
        {
            if (!loaders.ContainsKey(parent))
                loaders.Add(parent, new LoaderControl());

            loaders[parent].ShowLoader(parent);
        }

        public void ShowLoader(Control parent)
        {
            parent.SuspendLayout();

            parent.Controls.Add(this);

            Size = parent.Size;
            Location = new Point(0, 0);

            parent.Resize += ParentResize;

            BringToFront();

            parent.ResumeLayout();
        }

        private void ParentResize(object sender, EventArgs e)
        {
            Control parent = sender as Control;

            if (parent == null)
                return;

            Size = parent.Size;
        }

        public static void Hide(Control parent)
        {
            if (!loaders.ContainsKey(parent))
                return;

            loaders[parent].HideLoader(parent);
        }

        public void HideLoader(Control parent)
        {
            SuspendLayout();

            parent.Controls.Remove(this);

            parent.Resize -= ParentResize;

            ResumeLayout();
        }
    }
}

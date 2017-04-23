using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Tasks;
using System.Threading;
using MatthiWare.UpdateLib.UI;

namespace MatthiWare.UpdateLib.Generator.UI
{
    public partial class LoaderControl : UserControl
    {
        private static Dictionary<Control, LoaderControl> loaders = new Dictionary<Control, LoaderControl>();

        private const int WS_EX_TRANSPARENT = 0x20;

        private int m_opacity = 100;

        public int Opacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                m_opacity = value;

                if (m_opacity > 100)
                    m_opacity = 100;

                if (m_opacity < 0)
                    m_opacity = 1;

                var alpha = (m_opacity * 255) / 100;
                BackColor = Color.FromArgb(alpha, BackColor);
                Invalidate();
            }
        }


        public LoaderControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.Opaque, true);
            DoubleBuffered = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        public static void Show(Control parent)
        {
            if (parent == null)
                return;

            if (!loaders.ContainsKey(parent))
                loaders.Add(parent, new LoaderControl());

            loaders[parent].ShowLoader(parent);
        }

        public void ShowLoader(Control parent)
        {
            UIExtensions.InvokeOnUI(parent, (c) =>
            {
                parent.SuspendLayout();

                Opacity = 100;
                
                Size = parent.Size;
                //parent.Size = Size;
                Location = new Point(0, 0);

                c.Resize += ParentResize;

                c.Controls.Add(this);

                BringToFront();

                parent.ResumeLayout();
            });
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
            if (parent == null)
                return;

            if (!loaders.ContainsKey(parent))
                return;

            loaders[parent].HideLoader(parent);
        }

        public void HideLoader(Control parent)
        {
            UIExtensions.InvokeOnUI(parent, (c) =>
            {
                c.SuspendLayout();

                c.Resize -= ParentResize;

                c.Controls.Remove(this);
                
                c.ResumeLayout();
            });
        }
    }
}

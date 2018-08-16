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
using System.Drawing;
using System.Windows.Forms;
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
            Parent.InvokeOnUI(() =>
            {
                parent.SuspendLayout();

                Opacity = 100;
                
                Size = parent.Size;
                //parent.Size = Size;
                Location = new Point(0, 0);

                parent.Resize += ParentResize;

                parent.Controls.Add(this);

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
            Parent.InvokeOnUI(() =>
            {
                parent.SuspendLayout();

                parent.Resize -= ParentResize;

                parent.Controls.Remove(this);

                parent.ResumeLayout();
            });
        }
    }
}

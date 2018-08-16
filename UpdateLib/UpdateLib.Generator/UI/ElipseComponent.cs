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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI
{
    public class ElipseComponent : Component
    {
        #region PInvoke CreateRoundRectRgn

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int x, int y, int width, int height, int curveX, int curveY);

        private static void MakeRound(Form form, int elipse)
        {
            if (form == null)
                return;

            form.FormBorderStyle = FormBorderStyle.None;
            Region region = Region.FromHrgn(CreateRoundRectRgn(0, 0, form.Width, form.Height, elipse, elipse));
            form.Region = region;
        }

        #endregion

        private int m_radius;

        public int Radius
        {
            get { return m_radius; }
            set
            {
                m_radius = value;
                ApplyRadius();
            }
        }

        private ContainerControl m_control;

        public ContainerControl Control
        {
            get { return m_control; }
            set
            {
                if (m_control != null && m_control is Form)
                {
                    m_control.Resize -= M_control_Resize;
                }

                m_control = value;

                m_control.Resize += M_control_Resize;

                ApplyRadius();
            }
        }

        private void M_control_Resize(object sender, EventArgs e)
        {
            ApplyRadius();
        }

        public ElipseComponent()
        {
            m_radius = 5;
        }

        public ElipseComponent(IContainer container)
            : this()
        {
            container.Add(this);
        }

        public void ApplyRadius()
        {
            if (Control == null)
                return;

            if (!(Control is Form))
                return;

            MakeRound(Control as Form, Radius);
        }

        public override ISite Site
        {
            get
            {
                return base.Site;
            }

            set
            {
                base.Site = value;

                if (value == null)
                    return;

                IComponent rootComponent;
                Type serviceType = typeof(IDesignerHost);
                IDesignerHost service = Site.GetService(serviceType) as IDesignerHost;

                if (service == null)
                    return;

                rootComponent = service.RootComponent;

                if (!(rootComponent is ContainerControl))
                    return;

                Control = rootComponent as ContainerControl;
            }
        }
    }
}

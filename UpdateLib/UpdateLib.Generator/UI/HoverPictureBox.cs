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
using System.Drawing;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI
{
    public class HoverPictureBox : PictureBox
    {
        private int alpha = 255;
        private bool mouseInBox = false;
        private bool mouseDown = false;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!mouseInBox)
            {
                mouseInBox = true;
                SwitchBackgroundColor();
            } 
        }

        private void SwitchBackgroundColor()
        {
            alpha = (mouseInBox ? 100 : 0);
            alpha = (mouseDown ? 150 : alpha);
            BackColor = Color.FromArgb(alpha, Color.WhiteSmoke);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (!mouseInBox)
            {
                mouseInBox = true;
                SwitchBackgroundColor();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (mouseInBox)
            {
                mouseInBox = false;
                SwitchBackgroundColor();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!mouseInBox)
            {
                mouseInBox = true;
                SwitchBackgroundColor();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!mouseDown)
            {
                mouseDown = true;
                SwitchBackgroundColor();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (mouseDown)
            {
                mouseDown = false;
                SwitchBackgroundColor();
            }
        }

    }
}

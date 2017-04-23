using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

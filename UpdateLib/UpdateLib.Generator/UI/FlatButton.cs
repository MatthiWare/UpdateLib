using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateLib.Generator.UI
{
    [DefaultEvent(nameof(Click))]
    public class FlatButton : Control
    {
        private bool mouseInside = false;

        private Bitmap buffer;

        private bool m_activeItem;

        private Color m_backColor;

        public bool ActiveItem
        {
            get { return m_activeItem; }
            set { m_activeItem = value; UpdateBackgroundColor(); }
        }

        private Color m_hoveColor;

        public Color BackHoverColor
        {
            get { return m_hoveColor; }
            set { m_hoveColor = value; }
        }

        private Color m_selectedColor;

        public Color BackSelectedColor
        {
            get { return m_selectedColor; }
            set { m_selectedColor = value; }
        }


        public FlatButton()
            : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
        }

        protected override void OnClick(EventArgs e)
        {
            ActiveItem = true;

            foreach (Control c in Parent.Controls)
            {
                FlatButton button = c as FlatButton;

                if (button == null || button == this)
                    continue;

                button.ActiveItem = false;
            }

            base.OnClick(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!mouseInside)
            {
                mouseInside = true;
                UpdateBackgroundColor();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (mouseInside)
            {
                mouseInside = false;
                UpdateBackgroundColor();
            }
        }

        private void UpdateBackgroundColor()
        {
            m_backColor = (mouseInside ? m_hoveColor : BackColor);
            m_backColor = (m_activeItem ? m_selectedColor : m_backColor);

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            buffer = new Bitmap(Width, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, Width , Height );

            buffer = buffer ?? new Bitmap(Width, Height);

            using (Graphics g = Graphics.FromImage(buffer))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                g.Clear(Color.White);

                g.FillRectangle(new SolidBrush(m_backColor), rect);

                SizeF textSize = g.MeasureString(Text, Font, Width);

                float x = (Width / 2) - (textSize.Width / 2);
                float y = (Height / 2) - (textSize.Height / 2);

                g.DrawString(Text, Font, new SolidBrush(ForeColor), x, y);

                base.OnPaint(e);
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            }
        }
    }
}

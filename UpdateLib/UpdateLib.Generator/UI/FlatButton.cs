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
        private const float PADDING_WIDTH = 0.0625f;
        private const float PADDING_HEIGHT = 0.25f;
        private const float IMG_SIZE_WIDTH = 0.125f;
        private const float IMG_SIZE_HEIGHT = 0.5f;
        private const float TEXT_SIZE_WIDTH = 1f - (PADDING_WIDTH * 3) - IMG_SIZE_WIDTH;
        private const float TEXT_SIZE_HEIGHT = 1f - (PADDING_HEIGHT * 2);

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

        private Image m_infoImage;

        public Image InfoImage
        {
            get { return m_infoImage; }
            set
            {
                m_infoImage = value;

                Rectangle rect = new Rectangle(
                    (int)(Width * PADDING_WIDTH),
                    (int)(Height * PADDING_HEIGHT),
                    (int)(Width * IMG_SIZE_WIDTH),
                    (int)(Height * IMG_SIZE_HEIGHT));

                Invalidate(rect);
            }
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
            m_backColor = (m_activeItem ? m_selectedColor : BackColor);
            m_backColor = (mouseInside ? m_hoveColor : m_backColor);


            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            buffer = new Bitmap(Width, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            buffer = buffer ?? new Bitmap(Width, Height);

            using (Graphics g = Graphics.FromImage(buffer))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                g.Clear(Color.White);

                g.FillRectangle(new SolidBrush(m_backColor), rect);

                RectangleF imgRect = new RectangleF(
                    Width * PADDING_WIDTH,
                    Height * PADDING_HEIGHT,
                    Width * IMG_SIZE_WIDTH,
                    Height * IMG_SIZE_HEIGHT);

                if (InfoImage != null)
                    g.DrawImage(InfoImage, imgRect);

                SizeF textSize = g.MeasureString(Text, Font, (int)(Width * TEXT_SIZE_WIDTH));

                float offsetX = ((Width * PADDING_WIDTH) * 2) + imgRect.Width;
                float offsetY = imgRect.Y;

                float availableTextWidth = Width - offsetX - (Width * PADDING_WIDTH);
                float availableTextHeight = Height - (offsetY * 2);

                float x = offsetX + (availableTextWidth / 2) - (textSize.Width / 2);
                float y = offsetY + (availableTextHeight / 2) - (textSize.Height / 2);

                g.DrawString(Text, Font, new SolidBrush(ForeColor), x, y);

                base.OnPaint(e);
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            }
        }
    }
}

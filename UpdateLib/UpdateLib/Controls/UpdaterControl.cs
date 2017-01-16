using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Properties;

namespace MatthiWare.UpdateLib.Controls
{
    [ToolboxBitmap(typeof(UpdaterControl), "UpdaterControl.bmp")]
    public partial class UpdaterControl : UserControl
    {
        private Bitmap bmpInfo, bmpError, bmpDone, bmpUpdate;

        private const int ICON_SIZE = 24;
        private const int XY_OFFSET = 2;

        private Brush brush;
        private float x_text, y_text;

        private const string CHECK_FOR_UPDATES = "Please check for updates";

        private Point oldLocation;

        private Dictionary<string, SizeF> cachedMeasure = new Dictionary<string, SizeF>();
        private string text = CHECK_FOR_UPDATES;

        public UpdaterControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ContainerControl, false);

            Size = new Size(XY_OFFSET * 2 + ICON_SIZE, XY_OFFSET * 2 + ICON_SIZE);
            
            // caching
            LoadImages();
            MakeBrushFromForeColor();
            CalcFont();
           
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
                // invalidate the cache
                cachedMeasure.Clear(); 
                CalcFont();
            }
        }

        private void CalcFont()
        {
            SizeF size = GetAndCacheSize();
            
            int height = XY_OFFSET * 2 + ICON_SIZE;
            
            x_text = XY_OFFSET * 2 + ICON_SIZE;
            y_text = (height / 2) - (size.Height / 2);
        }

        private SizeF GetAndCacheSize()
        {
            if (!cachedMeasure.ContainsKey(text))
            {
                Graphics g = Graphics.FromImage(new Bitmap(100, 100));
                SizeF size = g.MeasureString(text, base.Font);

                cachedMeasure.Add(text, size);
            }

            return cachedMeasure[text];
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
                MakeBrushFromForeColor();
            }
        }

        private void MakeBrushFromForeColor()
        {
            brush = new SolidBrush(base.ForeColor);
        }

        private void LoadImages()
        {
            bmpInfo = Resources.status_info;
            bmpError = Resources.status_error;
            bmpDone = Resources.status_done;
            
            bmpUpdate = Resources.status_download;
            bmpUpdate.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawIcon(e.Graphics);
            DrawText(e.Graphics);  
        }

        private void DrawIcon(Graphics g)
        {
            g.DrawImage(bmpInfo, XY_OFFSET, XY_OFFSET, ICON_SIZE, ICON_SIZE);
        }

        private void DrawText(Graphics g)
        {
            g.DrawString(text, Font, brush, x_text, y_text);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            oldLocation = Location;
            int newWidth = CalcNewWidth();
            Width = newWidth;

            if (Location.X + newWidth > ParentForm.Width)
            {
                int amountToRemove = ParentForm.Width - (Location.X + newWidth) - (XY_OFFSET * 2 + ICON_SIZE);
                Point x = Location;
                x.X += amountToRemove;
                Location = x;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Location = oldLocation;
            Width = XY_OFFSET * 2 + ICON_SIZE;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            ToolTip tooltip = new ToolTip();
            tooltip.ToolTipIcon = ToolTipIcon.Info;
            tooltip.ToolTipTitle = text;
            tooltip.UseAnimation = true;
            tooltip.Show("Please click here to start checking for updates", this);

        }

        private int CalcNewWidth()
        {
            SizeF size = GetAndCacheSize();
            return (int)size.Width + (XY_OFFSET * 2 + ICON_SIZE);
        }

    }
}

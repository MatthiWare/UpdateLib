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

        private const string CHECK_FOR_UPDATES = "Check for updates";



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
                CalcFont();
            }
        }

        private void CalcFont()
        {
            Graphics g = Graphics.FromImage(new Bitmap(50,50));
            int height = XY_OFFSET * 2 + ICON_SIZE;
            SizeF size = g.MeasureString(CHECK_FOR_UPDATES, base.Font);

            x_text = XY_OFFSET * 2 + ICON_SIZE;
            y_text = (height / 2) - (size.Height / 2);
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
            g.DrawString("Please check for updates", Font, brush, x_text, y_text);
        }

        
    }
}

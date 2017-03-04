using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MatthiWare.UpdateLib.Properties;

namespace MatthiWare.UpdateLib.Controls
{
    [ToolboxBitmap(typeof(UpdaterControl), "UpdaterControl.bmp")]
    public partial class UpdaterControl : UserControl
    {
        private const int ICON_SIZE = 16;
        private const int XY_OFFSET = 2;
        private const int PROGRESS_SPEED = 200;

        private Brush brush;
        private float x_text, y_text;

        private const string CHECK_FOR_UPDATES = "Please check for updates";

        private Point oldLocation;
        private ToolTip tooltip = new ToolTip();

        private Dictionary<string, SizeF> cachedMeasure = new Dictionary<string, SizeF>();
        private string _text = CHECK_FOR_UPDATES;
        public override string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    Invalidate();
                }
                
            }
        }

        private int progressIndex = 0;
        private Bitmap[] progressImages = new Bitmap[50];

        private Dictionary<UpdaterIcon, Bitmap> cachedImages = new Dictionary<UpdaterIcon, Bitmap>();

        private Timer timer;

        private UpdaterIcon _icon = UpdaterIcon.Info;
        private UpdaterIcon Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    Invalidate();
                }
                
            }
        }

        public enum UpdaterIcon : byte
        {
            Info = 0,
            Error = 1,
            Done = 2,
            Update = 3,
            Progress = 4
        }

        public UpdaterControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ContainerControl, false);

            Size = new Size(XY_OFFSET * 2 + ICON_SIZE, XY_OFFSET * 2 + ICON_SIZE);
            timer = new Timer();
            timer.Interval = PROGRESS_SPEED;
            timer.Tick += update_progress;

            // caching
            LoadImages();
            MakeBrushFromForeColor();
            CalcFont();
           
        }

        private void update_progress(object sender, EventArgs e)
        {
            Invalidate();
            if (++progressIndex >= progressImages.Length)
                progressIndex = 0;
        }

        private void StartProgress()
        {
            progressIndex = 0;
            timer.Start();
        }

        private void StopProgress()
        {
            timer.Stop();
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
            if (!cachedMeasure.ContainsKey(Text))
            {
                Graphics g = Graphics.FromImage(new Bitmap(1,1));
                SizeF size = g.MeasureString(Text, base.Font);

                cachedMeasure.Add(Text, size);
            }

            return cachedMeasure[Text];
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
            Resources.status_download.RotateFlip(RotateFlipType.RotateNoneFlipY);
            cachedImages.Add(UpdaterIcon.Info, Resources.status_info);
            cachedImages.Add(UpdaterIcon.Error, Resources.status_error);
            cachedImages.Add(UpdaterIcon.Done, Resources.status_done);
            cachedImages.Add(UpdaterIcon.Update, Resources.status_update);

            Bitmap spritesheet = Resources.status_working;
            
            int i = 0;
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Bitmap bmp = new Bitmap(16, 16);
                    Graphics g = Graphics.FromImage(bmp);

                    Rectangle dest = new Rectangle(0, 0, 16, 16);
                    Rectangle src = new Rectangle(x * 16, y * 16, 16, 16);

                    g.DrawImage(spritesheet, dest, src, GraphicsUnit.Pixel);

                    g.Dispose();

                    progressImages[i++] = bmp;
                }
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawIcon(e.Graphics);
            DrawText(e.Graphics);  
        }

        private void DrawIcon(Graphics g)
        {
            g.DrawImage((Icon == UpdaterIcon.Progress) ? progressImages[progressIndex] : cachedImages[Icon], 
                XY_OFFSET, 
                XY_OFFSET, 
                ICON_SIZE, 
                ICON_SIZE);
        }

        private void DrawText(Graphics g)
        {
            g.DrawString(Text, Font, brush, x_text, y_text);
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

            tooltip.Active = false;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            
            tooltip.ToolTipIcon = ToolTipIcon.Info;
            tooltip.ToolTipTitle = Text;
            tooltip.UseAnimation = true;
            tooltip.Active = true;
            tooltip.Show("Please click here to start checking for updates", Parent, Location.X + Width + (ICON_SIZE), Location.Y + Height);

        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Text = "Checking for updates..";
            Icon = UpdaterIcon.Progress;

            int currWidth = Width;
            int newWidth = CalcNewWidth();

            int offset = currWidth - newWidth;
            Point x = Location;
            x.X += offset;
            Location = x;
            Width = newWidth;

            StartProgress();


        }

        private int CalcNewWidth()
        {
            SizeF size = GetAndCacheSize();
            return (int)size.Width + (XY_OFFSET * 2 + ICON_SIZE);
        }

    }
}

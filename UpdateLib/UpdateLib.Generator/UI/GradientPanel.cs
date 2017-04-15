using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.UI
{
    public class GradientPanel : Panel
    {

        public GradientPanel()
                : base()
        {

        }

        private int m_quality = 100;

        public int Quality
        {
            get { return m_quality; }
            set { m_quality = value; SetGradient(); }
        }


        private Color m_gradientTopRight = SystemColors.Control;

        public Color GradientTopRight
        {
            get { return m_gradientTopRight; }
            set { m_gradientTopRight = value; SetGradient(); }
        }

        private Color m_gradientTopLeft = SystemColors.Control;

        public Color GradientTopLeft
        {
            get { return m_gradientTopLeft; }
            set { m_gradientTopLeft = value; SetGradient(); }
        }

        private Color m_gradientBottomLeft = SystemColors.Control;

        public Color GradientBottomLeft
        {
            get { return m_gradientBottomLeft; }
            set { m_gradientBottomLeft = value; SetGradient(); }
        }

        private Color m_gradientBottomRight = SystemColors.Control;

        public Color GradientBottomRight
        {
            get { return m_gradientBottomRight; }
            set { m_gradientBottomRight = value; SetGradient(); }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            SetGradient();
        }

        private void SetGradient()
        {
            Bitmap buffer = new Bitmap(Quality, Quality);
            for (int x = 0; x < Quality; x++)
            {
                int percentX = (int)(((double)x / Width) * 100);

                Color c1 = GetColorScale(percentX, GradientTopLeft, GradientTopRight);

                for (int y = 0; y < Quality; y++)
                {
                    int percentY = (int)(((double)y / Height) * 100);

                    Color c2 = GetColorScale(percentY, GradientTopLeft, GradientTopRight);

                    buffer.SetPixel(x, y, DiffuseColors(c1, c2));
                }
            }

            if (BackgroundImageLayout != ImageLayout.Stretch)
                BackgroundImageLayout = ImageLayout.Stretch;

            SuspendLayout();

            BackgroundImage = buffer;

            ResumeLayout(true);
        }

        private Color GetColorScale(int percentage, Color start, Color end)
        {
            byte red = GetByte(percentage, start.R, end.R);
            byte green = GetByte(percentage, start.G, end.G);
            byte blue = GetByte(percentage, start.B, end.B);

            return Color.FromArgb(red, green, blue);
        }

        private byte GetByte(int percentage, byte begin, byte end)
        {
            return (byte)(Math.Round((begin + ((end - begin) * percentage) * 0.01), 0));
        }

        private Color DiffuseColors(Color a, Color b)
        {
            int red = (a.R + b.R) / 2;
            int green = (a.G + b.G) / 2;
            int blue = (a.B + b.B) / 2;
            return Color.FromArgb(red, green, blue);
        }


    }
}

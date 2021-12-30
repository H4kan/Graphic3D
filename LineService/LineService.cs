using MysteryProject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MysteryProject
{
    public class LineService
    {
        public DirectBitmap Bmp { get; set; }

        public BresenhamLine BrensehamLine { get; set; }

        public PictureBox PictureBox { get; set; }

        private Form1 form;

        public LineService(DirectBitmap bmp, PictureBox pictureBox, Form1 form)
        {
            this.Bmp = bmp;
            this.BrensehamLine = new BresenhamLine(bmp);

            this.PictureBox = pictureBox;
            this.form = form;
        }

        public void FastHorizontalLine(int x1, int x2, int y, Color color)
        {
            if (x1 >= Bmp.Width || x2 < 0 || y < 0 || y >= Bmp.Height) return;
            if (x1 < 0) x1 = 0;
            if (x2 >= Bmp.Width) x2 = Bmp.Width - 1;
            int x = x1;
            while (x <= x2)
            {
                Bmp.SetPixel(x, y, color);
                x++;
            }

        }
    }
}

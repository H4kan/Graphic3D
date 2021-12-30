using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MysteryProject
{
    public class BresenhamLine
    {
        public DirectBitmap bmp { get; set; }

        public BresenhamLine(DirectBitmap bmp)
        {
            this.bmp = bmp;
        }

        private void BresenhamLow(int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int y_step = 1;
            if (dy < 0)
            {
                y_step = -1;
                dy = -dy;
            }
            int d = (2 * dy) - dx;
            int y = y1;
            int x = x1;

            while (x <= x2)
            {
                if (y >= 0 && y < bmp.Height && x >= 0 && x < bmp.Width)
                    bmp.SetPixel(x, y, color);
                if (d > 0)
                {
                    y += y_step;
                    d += 2 * (dy - dx);
                }
                else
                {
                    d += 2 * dy;
                }
                x++;
            }
        }

        private void BresenhamHigh(int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int x_step = 1;
            if (dx < 0)
            {
                x_step = -1;
                dx = -dx;
            }
            int d = (2 * dx) - dy;
            int y = y1;
            int x = x1;

            while (y <= y2)
            {
                if (y >= 0 && y < bmp.Height && x >= 0 && x < bmp.Width)
                    bmp.SetPixel(x, y, color);
                if (d > 0)
                {
                    x += x_step;
                    d += 2 * (dx - dy);
                }
                else
                {
                    d += 2 * dx;
                }
                
                y++;
            }
        }

        public Line CreateLine(Point first, Point sec)
        {
            return this.CreateLine(first.X, first.Y, sec.X, sec.Y);
        }

        public void EraseLine(Point first, Point sec)
        {
            this.EraseLine(first.X, first.Y, sec.X, sec.Y);
        }

        private Line CreateLine(int x1, int y1, int x2, int y2)
        {
            var line = new Line();
            
            if (Math.Abs(y2 - y1) < Math.Abs(x2 - x1))
            {
                if (x1 > x2)
                    BresenhamLow(x2, y2, x1, y1, Color.Black);
                else
                    BresenhamLow(x1, y1, x2, y2, Color.Black);
            }
            else
            {
                if (y1 > y2)
                    BresenhamHigh(x2, y2, x1, y1, Color.Black);
                else
                    BresenhamHigh(x1, y1, x2, y2, Color.Black);
            }


            line.Start = new Point(x1, y1);
            line.End = new Point(x2, y2);

            return line;
        }

        private void EraseLine(int x1, int y1, int x2, int y2)
        {

            if (Math.Abs(y2 - y1) < Math.Abs(x2 - x1))
            {
                if (x1 > x2)
                    BresenhamLow(x2, y2, x1, y1, Color.White);
                else
                    BresenhamLow(x1, y1, x2, y2, Color.White);
            }
            else
            {
                if (y1 > y2)
                    BresenhamHigh(x2, y2, x1, y1, Color.White);
                else
                    BresenhamHigh(x1, y1, x2, y2, Color.White);
            }
            
        }
    }
}

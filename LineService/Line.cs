using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MysteryProject
{
    public class Line
    {

        public Point Start { get; set; }
        public Point End { get; set; }

        public Point EvaluateMidPoint()
        {
            return new Point((Start.X + End.X) / 2,
                (Start.Y + End.Y) / 2);
        }
    }
}

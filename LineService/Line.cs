using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class Line
    {

        public Point Start { get; set; }
        public Point End { get; set; }

        public Line()
        { }

        public Line(Vector3 start, Vector3 end)
        {
            Start = new Point(Convert.ToInt32(start.X), Convert.ToInt32(start.Y));
            End = new Point(Convert.ToInt32(end.X), Convert.ToInt32(end.Y));
        }

        public Point EvaluateMidPoint()
        {
            return new Point((Start.X + End.X) / 2,
                (Start.Y + End.Y) / 2);
        }
    }
}

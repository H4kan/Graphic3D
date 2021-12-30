using System;
using System.Collections.Generic;
using System.Text;

namespace MysteryProject
{
    public class EdgeHandler
    {
        public int yMax { get; set; }
        public int yMin { get; set; }

        public int xMin { get; set; }
        public int x { get; set; }
        
        public int dX { get; set; }
        public int dY { get; set; }

        public int basicX { get; set; }
        

        public EdgeHandler(Line line)
        {
            var smallerY = line.Start.Y > line.End.Y ? line.End : line.Start;
            var greaterY = line.Start.Y > line.End.Y ? line.Start : line.End;
            yMin = smallerY.Y;
            x = smallerY.X;
            basicX = x;
            yMax = greaterY.Y;
            xMin = Math.Min(smallerY.X, greaterY.X);
            
            dX = (greaterY.X - smallerY.X);
            dY = (greaterY.Y - smallerY.Y);
            
        }
    }
}

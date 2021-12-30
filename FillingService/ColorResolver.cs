using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class ColorResolver
    {

        Polygon triangle { get; set; }
        (Vector2, Vector2, Vector2) colorsInVertices { get; set; }
        float intensity { get; set; }

        DirectBitmap txtBmp { get; set; }
        
        public ColorResolver(Polygon triangle, DirectBitmap txtBmp, (Vector2, Vector2, Vector2) colorsInVecrtices, float intensity)
        {
            this.triangle = triangle;
            this.colorsInVertices = colorsInVecrtices;
            this.intensity = intensity;
            this.txtBmp = txtBmp;
        }

        public Color ResolveColor(Vector2 pointInTriangle)
        {
            var weights = Helpers.GetBaycentricCoords(triangle.Points, pointInTriangle);

            var coords = weights.X * colorsInVertices.Item1 + weights.Y * colorsInVertices.Item2 + weights.Z * colorsInVertices.Item3;

            var color = this.txtBmp.GetPixel(Convert.ToInt32(Math.Round(coords.X * this.txtBmp.Width)), Convert.ToInt32(Math.Round(coords.Y * this.txtBmp.Height)));

            return Color.FromArgb(Convert.ToInt32(Math.Round(intensity * color.R)), Convert.ToInt32(Math.Round(intensity * color.G)), Convert.ToInt32(Math.Round(intensity * color.B)));
        }

    }
}

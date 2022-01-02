using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class ColorResolver
    {
        (Vector2, Vector2, Vector2) colorsInVertices { get; set; }

        DirectBitmap txtBmp { get; set; }

        ShadingService ShadingService { get; set; }
        
        public ColorResolver(Polygon triangle, DirectBitmap txtBmp, (Vector2, Vector2, Vector2) colorsInVecrtices, ShadingService shadingService)
        {
            this.colorsInVertices = colorsInVecrtices;
            this.txtBmp = txtBmp;
            this.ShadingService = shadingService;
        }

        public Color ResolveColor(Vector3 weights, Vector3 point)
        {

            var coords = weights.X * colorsInVertices.Item1 + weights.Y * colorsInVertices.Item2 + weights.Z * colorsInVertices.Item3;

            var color = this.txtBmp.GetPixel(Math.Max(0, Math.Min(Convert.ToInt32(coords.X * this.txtBmp.Width), this.txtBmp.Height - 1)), Math.Max(0, Math.Min(Convert.ToInt32(coords.Y * this.txtBmp.Height), this.txtBmp.Width - 1)));

            var intensity = Math.Min(1, Math.Max(0, this.ShadingService.GetShading(weights, point, out float spec)));

            if (spec > 0)
            {
                var r = Convert.ToInt32(Math.Min(5 + color.R * (intensity + 1.3 * spec), 255));
                var g = Convert.ToInt32(Math.Min(5 + color.G * (intensity + 1.3 * spec), 255));
                var b = Convert.ToInt32(Math.Min(5 + color.B * (intensity + 1.3 * spec), 255));


                return Color.FromArgb(r, g, b);
            }

            return Color.FromArgb(Math.Min(255, Convert.ToInt32(intensity * color.R)), Math.Min(255, Convert.ToInt32(intensity * color.G)), Math.Min(255, Convert.ToInt32(intensity * color.B)));
        }

    }
}

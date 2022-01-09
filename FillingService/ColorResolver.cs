using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class ColorResolver
    {
        ShadingService ShadingService { get; set; }

        ITextureProvider TextureProvider { get; set; }
        
        public ColorResolver(ShadingService shadingService, ITextureProvider textureProvider)
        {
            this.ShadingService = shadingService;
            this.TextureProvider = textureProvider;
        }

        public Color ResolveColor(Vector3 weights, Vector3 point)
        {

            //var coords = weights.X * colorsInVertices.Item1 + weights.Y * colorsInVertices.Item2 + weights.Z * colorsInVertices.Item3;

            //var color = this.txtBmp.GetPixel(Math.Max(0, Math.Min(Convert.ToInt32(coords.X * this.txtBmp.Width), this.txtBmp.Height - 1)), Math.Max(0, Math.Min(Convert.ToInt32(coords.Y * this.txtBmp.Height), this.txtBmp.Width - 1)));

            var color = TextureProvider.ResolveColor(point);

            var intensity = Math.Max(0, this.ShadingService.GetShading(weights, point, out bool spec));

            if (spec)
            {
                var r = Convert.ToInt32(Math.Min(5 + color.R * intensity, 255));
                var g = Convert.ToInt32(Math.Min(5 + color.G * intensity, 255));
                var b = Convert.ToInt32(Math.Min(5 + color.B * intensity, 255));


                return Color.FromArgb(r, g, b);
            }

            return Color.FromArgb(Math.Min(255, Convert.ToInt32(intensity * color.R)), Math.Min(255, Convert.ToInt32(intensity * color.G)), Math.Min(255, Convert.ToInt32(intensity * color.B)));
        }

    }
}

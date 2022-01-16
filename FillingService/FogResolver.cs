using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MysteryProject
{
    public class FogResolver
    {
        private Color fogColor;
        private float minDist;

        public FogResolver(Color fogColor, float minDist)
        {
            this.fogColor = fogColor;
            this.minDist = minDist;
        }

        public Color ResolveFogColor(Color color, float distance)
        {
            if (distance < minDist)
                return color;
            else
            {
                float ratioFog = (distance - minDist) / distance;
                float ratioColor = minDist / distance;
                return Color.FromArgb(
                    Math.Min(Convert.ToInt32(color.R * ratioColor + fogColor.R * ratioFog), 255),
                    Math.Min(Convert.ToInt32(color.G * ratioColor + fogColor.G * ratioFog), 255),
                    Math.Min(Convert.ToInt32(color.B * ratioColor + fogColor.B * ratioFog), 255)
                    ); 
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public interface ITextureProvider
    {
        Color ResolveColor(Vector3 point);
    }

    // this one is single color material, taken from real ball rendered in blender
    // amplified brightness by 10x bc it was very dark
    public class BowlingBallProvider : ITextureProvider
    {
        public Color ResolveColor(Vector3 point)
        {
            return Color.FromArgb(Convert.ToInt32(0.002273 * 2550), Convert.ToInt32(0.004677 * 2550), Convert.ToInt32(0.037410 * 2550));
        }
    }

    public class BowlingPinProvider : ITextureProvider
    {
        public Color ResolveColor(Vector3 point)
        {
            return Color.FromArgb(Convert.ToInt32(0.8 * 255), Convert.ToInt32(0.8 * 255), Convert.ToInt32(0.8 * 255));
        }
    }
}

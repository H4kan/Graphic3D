using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public interface ISpecularProvider
    {
        float ResolveSpecular(Vector3 reflect);
    }

    // this one is single density specular, taken from real ball rendered in blender, not sure if blender shown this one right to me
    // amplified brightness by 10x bc it was very dark
    public class BowlingBallSpecularProvider : ISpecularProvider
    {
        public float ResolveSpecular(Vector3 reflected)
        {
            return (float)Math.Pow(Math.Max(0, reflected.Z), Color.FromArgb(Convert.ToInt32(0.5 * 255), Convert.ToInt32(0.5 * 255), Convert.ToInt32(0.5 * 255)).R);
        }
    }
}

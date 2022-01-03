using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public static class Helpers
    {
        public static Vector3 GetBaycentricCoords(List<Vector3> triangle, Vector2 point)
        {
            var u = Vector3.Cross(
                new Vector3(triangle[2].X - triangle[0].X,
                            triangle[1].X - triangle[0].X,
                            triangle[0].X - point.X),
                new Vector3(triangle[2].Y - triangle[0].Y,
                            triangle[1].Y - triangle[0].Y,
                            triangle[0].Y - point.Y));

            if (Math.Abs(u.Z) < 1) return new Vector3(-1, -1, -1);

            return new Vector3(1 - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
        }

        public static Vector3 Vector4ToVector3(Vector4 v)
        {
            return new Vector3(
                v.X / v.W,
                v.Y / v.W,
                v.Z / v.W
                );
        }

        public static Vector4 Vector3ToVector4(Vector3 v)
        {
            return new Vector4(
                v.X, v.Y, v.Z, 1
                );
        }

    }
}

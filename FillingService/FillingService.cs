using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class FillingService
    {
        private LineService lineService;
        private DirectBitmap Bmp { get; set; }

        public bool isLightningApplied = false;


        public FillingService(LineService lineService, DirectBitmap bmp)
        {
            this.lineService = lineService;
            this.Bmp = bmp;
        }

        public List<EdgeHandler> InitTables(Polygon polygon)
        {
            var edgeTable = new List<EdgeHandler>();
            edgeTable.Clear();

            foreach (var line in polygon.Edges.Where(e => e != null))
            {
                edgeTable.Add(new EdgeHandler(line));
            }
            edgeTable = edgeTable.OrderBy(e => e.yMin).ToList();

            return edgeTable; 
        }


        public void RunFilling(List<Vector3> triangle, float[,] zMax, ColorResolver colorResolver)
        {
            // evaluating bounding box
            Vector2 boundingMin = new Vector2(this.Bmp.Width, this.Bmp.Height);
            Vector2 boundingMax = new Vector2(0, 0);
            for (int i = 0; i < 3; i++)
            {
                boundingMin.X = Math.Max(0, Math.Min(boundingMin.X, Convert.ToInt32(triangle[i].X)));
                boundingMin.Y = Math.Max(0, Math.Min(boundingMin.Y, Convert.ToInt32(triangle[i].Y)));

                boundingMax.X = Math.Min(this.Bmp.Width - 1, Math.Max(boundingMax.X, Convert.ToInt32(triangle[i].X)));
                boundingMax.Y = Math.Min(this.Bmp.Height - 1, Math.Max(boundingMax.Y, Convert.ToInt32(triangle[i].Y)));
            }

            Vector3 point = new Vector3();

            for (point.X = boundingMin.X; point.X <= boundingMax.X; point.X++)
                for (point.Y = boundingMin.Y; point.Y <= boundingMax.Y; point.Y++)
                {
                    Vector3 bayCoords = Helpers.GetBaycentricCoords(triangle, new Vector2(point.X, point.Y));
                    if (bayCoords.X >= 0 && bayCoords.Y >= 0 && bayCoords.Z >= 0)
                    {
                        point.Z = triangle[0].Z * bayCoords.X
                            + triangle[1].Z * bayCoords.Y 
                            + triangle[2].Z * bayCoords.Z;
                        if (zMax[Convert.ToInt32(point.X), Convert.ToInt32(point.Y)] < point.Z)
                        {
                            zMax[Convert.ToInt32(point.X), Convert.ToInt32(point.Y)] = point.Z;
                            this.Bmp.SetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y), 
                                colorResolver.ResolveColor(bayCoords, point));
                        }
                    }

                }
        }
    }
}

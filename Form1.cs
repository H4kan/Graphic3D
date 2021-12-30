using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MysteryProject
{
    public partial class Form1 : Form
    {
        public BresenhamLine BresenhamLine { get; set; }

        public LineService LineService { get; set; }

        public FillingService FillingService { get; set; }

        public DirectBitmap Bmp { get; set; }

        public DirectBitmap TxtBmp { get; set; }

        public double alpha { get; set; }

        public List<Matrix<double>> vectors { get; set; }

        public List<(int, int)> neighbourings { get; set; }

        public System.Threading.Timer timer { get; set; }

        public List<Vector3> originalVertices = new List<Vector3>();

        public List<Vector3> vertices = new List<Vector3>();

        public List<(int, int, int)> triangles = new List<(int, int, int)>();

        public List<Polygon> polygons = new List<Polygon>();

        public List<Vector2> textureVertices = new List<Vector2>();

        public List<(int, int, int)> textureIndexes = new List<(int, int, int)>();

        public Form1()
        {
            InitializeComponent();

            this.Width = 1000;
            this.Height = 1000;
            this.pictureBox.Width = 900;
            this.pictureBox.Height = 900;
            this.Bmp = new DirectBitmap(this.pictureBox.Width, this.pictureBox.Height);

            using (var txtBmpHandler = Image.FromFile("../../../african_head_diffuse.jpg"))
            {
                this.TxtBmp = new DirectBitmap(txtBmpHandler.Width, txtBmpHandler.Height);
                using (var graphics = Graphics.FromImage(this.TxtBmp.Bitmap))
                {
                    graphics.DrawImage(txtBmpHandler, 0, 0);
                }
            }

            this.pictureBox.Image = this.Bmp.Bitmap;

            this.BresenhamLine = new BresenhamLine(this.Bmp);
            this.LineService = new LineService(this.Bmp, this.pictureBox, this);
            this.FillingService = new FillingService(this.LineService, this.Bmp);

            string[] data = File.ReadAllLines("../../../african_head.obj");

            int size = 600;
            int pos = 450;


            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Length < 2) continue;
                var values = data[i].Split(" ");
                if (data[i][0] == 'v' && data[i][1] == ' ')
                {
                    originalVertices.Add(new Vector3(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)));
                    vertices.Add(new Vector3(Convert.ToInt32(Math.Round(pos + originalVertices[i].X * (size / 2))), Convert.ToInt32(Math.Round(pos + originalVertices[i].Y * (size / 2))), originalVertices[i].Z));
                }
                if (data[i][0] == 'f')
                {
                    triangles.Add((Int32.Parse((values[1].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[2].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[3].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1));
                    textureIndexes.Add((Int32.Parse((values[1].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[2].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[3].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1));
                }
                if (data[i][0] == 'v' && data[i][1] == 't')
                {
                    textureVertices.Add(new Vector2(float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat)));
                }
            }

            float[,] zMax = new float[this.Bmp.Width, this.Bmp.Height];
            for (int j = 0; j < triangles.Count; j++)
            {
                var pt1 = vertices[triangles[j].Item1];
                var pt2 = vertices[triangles[j].Item2];
                var pt3 = vertices[triangles[j].Item3];

                var poly = new Polygon();
                this.polygons.Add(poly);

                poly.Edges.Add(new Line(pt1, pt2));
                poly.Edges.Add(new Line(pt2, pt3));
                poly.Edges.Add(new Line(pt3, pt1));

                poly.Points.Add(pt1);
                poly.Points.Add(pt2);
                poly.Points.Add(pt3);

                Vector3 v1 = new Vector3(originalVertices[triangles[j].Item2].X - originalVertices[triangles[j].Item1].X, 
                    originalVertices[triangles[j].Item2].Y - originalVertices[triangles[j].Item1].Y,
                    originalVertices[triangles[j].Item2].Z - originalVertices[triangles[j].Item1].Z);
                Vector3 v2 = new Vector3(originalVertices[triangles[j].Item3].X - originalVertices[triangles[j].Item1].X,
                    originalVertices[triangles[j].Item3].Y - originalVertices[triangles[j].Item1].Y,
                    originalVertices[triangles[j].Item3].Z - originalVertices[triangles[j].Item1].Z);

                var normalVec = Vector3.Normalize(Vector3.Cross(v1, v2));

                Vector3 lightDir = new Vector3(0, 0, -1);

                var intensity = Vector3.Dot(normalVec, lightDir);



                var rnd = new Random();
                if (intensity > 0)
                {
                    var color = Color.FromArgb(Convert.ToInt32(Math.Round(intensity * 255)), Convert.ToInt32(Math.Round(intensity * 255)), Convert.ToInt32(Math.Round(intensity * 255)));
                    //this.FillingService.RunFilling(this.FillingService.InitTables(poly), Color.FromArgb(Convert.ToInt32(Math.Round(intensity * 255)), Convert.ToInt32(Math.Round(intensity * 255)), Convert.ToInt32(Math.Round(intensity * 255))));
                    var colorResolver = new ColorResolver(polygons[j], this.TxtBmp,
                        (
                        textureVertices[textureIndexes[j].Item1],
                        textureVertices[textureIndexes[j].Item2],
                        textureVertices[textureIndexes[j].Item3]
                        ), intensity);
                    this.FillingService.RunFilling(poly.Points, zMax, colorResolver);
                }
                
            }
            this.pictureBox.Invalidate();
            //this.CreateCube(1);

            //timer = new System.Threading.Timer(this.RefreshCube, null, 0, 100);
        }


        public Matrix<double> PMatrix(int w, int h)
        {
            var res = DenseMatrix.Create(4, 4, 0);
            res[0, 0] = h / (double)w;
            res[1, 1] = 1;
            res[2, 3] = 1;
            res[3, 2] = -1;

            return res;
        }

        public Matrix<double> TMatrix(int x, int y, int z)
        {
            var res = DenseMatrix.CreateIdentity(4);
            res[0, 3] = x;
            res[1, 3] = y;
            res[2, 3] = z;

            return res;
        }

        public Matrix<double> RMatrix(double alpha)
        {
            var res = DenseMatrix.CreateIdentity(4);
            res[0, 0] = Math.Cos(alpha);
            res[2, 2] = Math.Cos(alpha);
            res[0, 2] = -Math.Sin(alpha);
            res[2, 0] = Math.Sin(alpha);

            return res;
        }

        public Matrix<double> TransformMatrix(int cameraDist, double alpha)
        {
            return PMatrix(this.Bmp.Width, this.Bmp.Height) * TMatrix(0, 0, cameraDist) * RMatrix(alpha);
        }

        public void CreateCube(double A)
        {
            this.alpha = 0.0;

            List<Matrix<double>> vectors = new List<Matrix<double>>();

            vectors.Add(DenseVector.OfArray(new double[]{ -A, A, -A, 1 }).ToColumnMatrix());
            vectors.Add(DenseVector.OfArray(new double[] { -A, -A, -A, 1 }).ToColumnMatrix());
            vectors.Add(DenseVector.OfArray(new double[] { A, -A, -A, 1 }).ToColumnMatrix());
            vectors.Add(DenseVector.OfArray(new double[] { A, A, -A, 1 }).ToColumnMatrix());


            vectors.Add(DenseVector.OfArray(new double[] { -A, A, A, 1 }).ToColumnMatrix());
            vectors.Add(DenseVector.OfArray(new double[] { -A, -A, A, 1 }).ToColumnMatrix());
            vectors.Add(DenseVector.OfArray(new double[] { A, -A, A, 1 }).ToColumnMatrix());
            vectors.Add(DenseVector.OfArray(new double[] { A, A, A, 1 }).ToColumnMatrix());

            this.vectors = vectors;

            List<(int, int)> neighbourings = new List<(int, int)>();
            
            neighbourings.Add((0, 1));
            neighbourings.Add((1, 2));
            neighbourings.Add((2, 3));
            neighbourings.Add((0, 3));

            neighbourings.Add((4, 5));
            neighbourings.Add((5, 6));
            neighbourings.Add((6, 7));
            neighbourings.Add((4, 7));

            neighbourings.Add((0, 4));
            neighbourings.Add((1, 5));
            neighbourings.Add((2, 6));
            neighbourings.Add((3, 7));

            this.neighbourings = neighbourings;

        }

        public void RefreshCube(object state)
        {
            this.alpha += 0.1;
            this.alpha %= Math.PI;

            var transformMatrix = TransformMatrix(5, alpha);
            var transformedPoints = vectors.Select(v => Get2DPoint(v, transformMatrix)).ToList();


            this.pictureBox.Invoke((MethodInvoker)delegate {
                foreach (var neighbour in neighbourings)
                {
                    this.BresenhamLine.CreateLine(new Point(
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item1][0, 0])),
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item1][1, 0]))),
                    new Point(
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item2][0, 0])),
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item2][1, 0])))
                    );
                }
            });

            this.pictureBox.Invalidate();
            Thread.Sleep(50);


            this.pictureBox.Invoke((MethodInvoker)delegate
            {
                foreach (var neighbour in neighbourings)
                {
                    this.BresenhamLine.EraseLine(new Point(
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item1][0, 0])),
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item1][1, 0]))),
                    new Point(
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item2][0, 0])),
                    Convert.ToInt32(Math.Round(transformedPoints[neighbour.Item2][1, 0])))
                    );
                }
            });
            
        }

        public Matrix<double> Get2DPoint(Matrix<double> vec, Matrix<double> transformMatrix)
        {
            var v_c = transformMatrix * vec;
            var normalized = v_c / v_c[3, 0];


            var vec2D = DenseVector.OfArray(new double[] { this.Bmp.Height * (1 - normalized[1, 0]) / (double)2, this.Bmp.Width * (1 + normalized[0, 0]) / (double)2 }).ToColumnMatrix();

            return vec2D;
        }

    }
}

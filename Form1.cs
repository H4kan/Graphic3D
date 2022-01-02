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

        public ShadingService ShadingService { get; set; }

        public MatrixProvider MatrixProvider { get; set; }

        public DirectBitmap Bmp { get; set; }

        public DirectBitmap TxtBmp { get; set; }

        public DirectBitmap SpecularBmp { get; set; }

        public double alpha { get; set; }

        public List<Matrix<double>> vectors { get; set; }

        public List<(int, int)> neighbourings { get; set; }

        public System.Threading.Timer timer { get; set; }

        public List<Vector3> originalVertices = new List<Vector3>();

        public List<string> materialsTriangles = new List<string>();

        public List<(string, DirectBitmap)> materialNames = new List<(string, DirectBitmap)>();

        public List<Vector3> vertices = new List<Vector3>();

        public List<Vector3> normalForVertices = new List<Vector3>();

        public List<(int, int, int)> triangles = new List<(int, int, int)>();

        public List<Polygon> polygons = new List<Polygon>();

        public List<Vector2> textureVertices = new List<Vector2>();

        public List<(int, int, int)> textureIndexes = new List<(int, int, int)>();

        public Vector3 light = Vector3.Normalize(new Vector3(0, 0, 1));

        public Vector3 camera = new Vector3(0, 0, 5);

        public Vector3 eye = new Vector3(0, 0, 1);

        public Vector3 center = new Vector3(0, 0, 0);

        public Matrix4x4 projection, viewport, viewmodel;

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.timer = new System.Threading.Timer(this.Render, null, 0, 500);

            this.Render(null);

        }

        public Form1()
        {
            InitializeComponent();

            this.Width = 1000;
            this.Height = 1000;
            this.pictureBox.Width = 900;
            this.pictureBox.Height = 900;
            this.Bmp = new DirectBitmap(this.pictureBox.Width, this.pictureBox.Height);

            using (var txtBmpHandler = Image.FromFile("../../../marble.png"))
            {
                this.TxtBmp = new DirectBitmap(txtBmpHandler.Height, txtBmpHandler.Width);
                using (var graphics = Graphics.FromImage(this.TxtBmp.Bitmap))
                {
                    graphics.DrawImage(txtBmpHandler, 0, 0);
                }
            }
            this.SpecularBmp = null;
            //using (var txtBmpHandler = Image.FromFile("../../../african_head_spec.jpg"))
            //{
            //    this.SpecularBmp = new DirectBitmap(txtBmpHandler.Width, txtBmpHandler.Height);
            //    using (var graphics = Graphics.FromImage(this.SpecularBmp.Bitmap))
            //    {
            //        graphics.DrawImage(txtBmpHandler, 0, 0);
            //    }
            //}


            this.pictureBox.Image = this.Bmp.Bitmap;

            this.BresenhamLine = new BresenhamLine(this.Bmp);
            this.LineService = new LineService(this.Bmp, this.pictureBox, this);
            this.FillingService = new FillingService(this.LineService, this.Bmp);
            this.ShadingService = new ShadingService();
            this.MatrixProvider = new MatrixProvider();

            this.ShadingService.Type = ShadingType.Constant;
            this.ShadingService.SpecularBmp = this.SpecularBmp;

            string[] materials = File.ReadAllLines("../../../material.lib");
            int r = 0;
            while (r < materials.Length)
            {
                if (materials[r].StartsWith("newmtl"))
                {
                    var values = materials[r].Split(" ");

                    while (r < materials.Length && !materials[r].StartsWith("\tmap_Kd"))
                    {
                        r++;
                    }
                    if (r == materials.Length) break;
                    var values2 = materials[r].Split(" ");
                    using (var txtBmpHandler = Image.FromFile($"../../../textures/{values2[1]}"))
                    {
                        var bmp = new DirectBitmap(txtBmpHandler.Width, txtBmpHandler.Height);
                        using (var graphics = Graphics.FromImage(bmp.Bitmap))
                        {
                            graphics.DrawImage(txtBmpHandler, 0, 0);
                        }
                        //for (int i = 0; i < bmp.Height / 2; i++)
                        //{
                        //    for (int j = 0; j < bmp.Width; j++)
                        //    {
                        //        var bit = bmp.Bits[bmp.Height * i + j];
                        //        bmp.Bits[bmp.Height * i + j] = bmp.Bits[(bmp.Width - 1 - i) * bmp.Height + j];
                        //        bmp.Bits[(bmp.Width - 1 - i) * bmp.Height + j] = bit;
                        //    }
                        //}
                        materialNames.Add((values[1], bmp));
                    }
          
                }
                r++;
            }

            string[] data = File.ReadAllLines("../../../gurl6.obj");

            int size = 350;
            int posX = 450;
            int posY = 0;

            this.projection = this.MatrixProvider.ProjectionMatrix(camera);
            this.viewport = this.MatrixProvider.ViewportnMatrix(posX, posY, size);
            this.viewmodel = this.MatrixProvider.ViewModelMatrix(eye, center, new Vector3(0, 1, 0));
            string selectedFileName = "";
        
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Length < 2) continue;
                var values = data[i].Split(" ");
                if (data[i][0] == 'v' && data[i][1] == ' ')
                {
                    if (values.Length == 4)
                        originalVertices.Add(new Vector3(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)));
                    else
                        originalVertices.Add(new Vector3(float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                         float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                         float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat)));

                }
                if (data[i][0] == 'f')
                {
                    triangles.Add((Int32.Parse((values[1].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[2].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[3].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1));
                    textureIndexes.Add((Int32.Parse((values[1].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[2].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[3].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1));
                    materialsTriangles.Add(selectedFileName);
                }
                if (data[i][0] == 'v' && data[i][1] == 't')
                {
                    if (values.Length == 3)
                        textureVertices.Add(new Vector2(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat)));
                    else
                        textureVertices.Add(new Vector2(float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat)));
                }
                if (data[i][0] == 'v' && data[i][1] == 'n')
                {
                    //if (values.Length == 4)
                    //    normalForVertices.Add(new Vector3(float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                    //    float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                    //    float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)));
                    //else
                    //    normalForVertices.Add(new Vector3(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                    //        float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                    //        float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat)));
                }
                if (data[i].StartsWith("usemtl"))
                {
                    var material = materialNames.FirstOrDefault(m => m.Item1 == values[1]);
                    if (material != default((string, DirectBitmap)))
                    {
                        selectedFileName = material.Item1;
                    }
                    else selectedFileName = "";
                }
            }
            this.viewmodel = this.MatrixProvider.ViewModelMatrix(eye, center, new Vector3(0, 1, 0));

          
            //this.pictureBox.Invalidate();
        }

        public void Render(object state)
        {

            this.pictureBox.Invoke((MethodInvoker)delegate {
                for (int i = 0; i < this.Bmp.Bits.Length; i++)
                    this.Bmp.Bits[i] = Color.Black.ToArgb();
                this.eye.Y += 0.5f;
                this.viewmodel = this.MatrixProvider.ViewModelMatrix(eye, center, new Vector3(0, 1, 0));

                var transformation = this.viewmodel * this.projection * this.viewport;
                vertices.Clear();
                for (int i = 0; i < originalVertices.Count; i++)
                {
                    var screenVertice = Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(originalVertices[i]), transformation));
                    vertices.Add(screenVertice);
                }
                Matrix4x4.Invert(Matrix4x4.Transpose(this.viewmodel * this.projection), out var normalTransformation);
                //for (int i = 0; i < triangles.Count; i++)
                //{
                //    Vector3 sum = new Vector3(0, 0, 0);
                //    var neighbours = Helpers.findNeighbours(triangles, triangles[i]);
                //    foreach (var triangle in neighbours)
                //    {

                //        Vector3 v1 = new Vector3(vertices[triangle.Item2].X - vertices[triangle.Item1].X,
                //               vertices[triangle.Item2].Y - vertices[triangle.Item1].Y,
                //               vertices[triangle.Item2].Z - vertices[triangle.Item1].Z);
                //        Vector3 v2 = new Vector3(vertices[triangle.Item3].X - vertices[triangle.Item1].X,
                //            vertices[triangle.Item3].Y - vertices[triangle.Item1].Y,
                //            vertices[triangle.Item3].Z - vertices[triangle.Item1].Z);

                //        sum  += Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(Vector3.Cross(v2, v1)), normalTransformation)));
                //    }
                //    normalForVertices.Add(sum / 3);
                //}



                float[,] zMax = new float[this.Bmp.Width, this.Bmp.Height];
                for (int i = 0; i < this.Bmp.Width; i++)
                    for (int j = 0; j < this.Bmp.Height; j++)
                    {
                        zMax[i, j] = float.NegativeInfinity;
                    }
                for (int j = 0; j < triangles.Count; j++)
                {
                    if (materialsTriangles[j].Length > 0)
                    {
                        this.TxtBmp = materialNames.First(m => m.Item1 == materialsTriangles[j]).Item2;
                    }
                    var pt1 = vertices[triangles[j].Item1];
                    var pt2 = vertices[triangles[j].Item2];
                    var pt3 = vertices[triangles[j].Item3];

                    var poly = new Polygon();
                    poly.Edges.Add(new Line(pt1, pt2));
                    poly.Edges.Add(new Line(pt2, pt3));
                    poly.Edges.Add(new Line(pt3, pt1));

                    poly.Points.Add(pt1);
                    poly.Points.Add(pt2);
                    poly.Points.Add(pt3);

                    var shouldRender = this.ShadingService.SetShading(triangles[j],
                        light,
                        vertices, normalForVertices, normalTransformation);

                    if (shouldRender)
                    {
                        var colorResolver = new ColorResolver(poly, this.TxtBmp,
                            (
                            textureVertices[textureIndexes[j].Item1],
                            textureVertices[textureIndexes[j].Item2],
                            textureVertices[textureIndexes[j].Item3]
                            ), this.ShadingService);
                        this.FillingService.RunFilling(poly.Points, zMax, colorResolver);

                    }

                }
                this.pictureBox.Invalidate();
            });
        }
    }
}

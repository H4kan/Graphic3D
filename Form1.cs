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

        public DirectBitmap SpecularBmp { get; set; }

        public System.Threading.Timer timer { get; set; }

        public List<Vector3> originalVertices = new List<Vector3>();

        public List<Vector3> vertices = new List<Vector3>();

        public List<Vector3> normalForVertices = new List<Vector3>();

        public List<(int, int, int)> triangles = new List<(int, int, int)>();

        //public List<Vector2> textureVertices = new List<Vector2>();

        //public List<(int, int, int)> textureIndexes = new List<(int, int, int)>();

        public Vector3 light = Vector3.Normalize(new Vector3(0, 0, 0.5f)); 

        public Vector3 camera = new Vector3(0, 0, 1.0f);

        public Vector3 eye = new Vector3(1, 0, 1);

        public Vector3 center = new Vector3(0, 0, 0);

        public Matrix4x4 projection, viewport, viewmodel;

        public ITextureProvider BowlingBallProvider { get; set; }

        public ISpecularProvider BowlingBallSpecularProvider { get; set; }

        public List<RenderSubject> Subjects { get; set; }

        public bool globalChange = true;

        public int frameCounter = 10;

        public bool preventRendering = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.timer = new System.Threading.Timer(this.Render, null, 0, 1000 / frameCounter);

        }

        public Form1()
        {
            InitializeComponent();

            this.Width = 1000;
            this.Height = 1000;
            this.pictureBox.Width = 900;
            this.pictureBox.Height = 900;
            this.Bmp = new DirectBitmap(this.pictureBox.Width, this.pictureBox.Height);
            this.pictureBox.Image = this.Bmp.Bitmap;

            this.BresenhamLine = new BresenhamLine(this.Bmp);
            this.LineService = new LineService(this.Bmp, this.pictureBox, this);
            this.FillingService = new FillingService(this.LineService, this.Bmp);
            this.ShadingService = new ShadingService();
            this.MatrixProvider = new MatrixProvider();

            this.ShadingService.Type = ShadingType.Constant;
            this.ShadingService.SpecularBmp = this.SpecularBmp;

            this.BowlingBallProvider = new BowlingPinProvider();
            this.BowlingBallSpecularProvider = new BowlingBallSpecularProvider();

            this.projection = this.MatrixProvider.ProjectionMatrix(camera);
            this.viewmodel = this.MatrixProvider.ViewModelMatrix(eye, center, new Vector3(0, 1, 0));

            this.Subjects = new List<RenderSubject>();

            var pinPath = "../../../pin.obj";
            ReadVertices(pinPath);
            for (int i = 0; i < 3; i++)
            {
                var pin = new RenderSubject();
                pin.size = 800;
                pin.posY = 400;
                pin.posX = 500 - i * 50;

                pin.angle = 0.5f;
                pin.diffAngle = 0;
                pin.translationPosition = new Vector3(0, 0, -0.5f);
                pin.diffPosition = new Vector3(0, 0, 0);
                pin.needRecalculation = false;
                pin.RefreshMatrices(MatrixProvider);

                pin.textureProvider = new BowlingPinProvider();
                pin.specularProvider = new BowlingBallSpecularProvider();

                pin.originalVertices.AddRange(this.originalVertices);
                pin.triangles.AddRange(this.triangles);
                pin.normals.AddRange(this.normalForVertices);

                pin.InitMatrices(MatrixProvider);
                this.Subjects.Add(pin);
            }

            var ballPath = "../../../BowlingBall2.obj";
            this.originalVertices.Clear();
            this.triangles.Clear();
            this.normalForVertices.Clear();
            ReadVertices(ballPath);

            var ball = new RenderSubject();
            ball.size = 800;
            ball.posX = 450;
            ball.posY = 450;

            ball.baseAngle = 0;
            ball.angle = 0;
            ball.diffAngle = 0.5f;
            ball.baseTranslationPosition = new Vector3(0, 0, 0.75f);
            ball.translationPosition = new Vector3(0, 0, 0.75f);
            ball.diffPosition = new Vector3(0, 0, -0.05f);

            ball.frameRestart = 50;
            ball.needRecalculation = true;
            ball.specularProvider = new BowlingBallSpecularProvider();
            ball.textureProvider = new BowlingBallProvider();

            ball.originalVertices.AddRange(this.originalVertices);
            ball.triangles.AddRange(this.triangles);
            ball.normals.AddRange(this.normalForVertices);

            ball.InitMatrices(MatrixProvider);
            this.Subjects.Add(ball);



            this.viewmodel = this.MatrixProvider.ViewModelMatrix(eye, center, new Vector3(0, 1, 0));
        }

        public void Render(object state)
        {
            if (this.preventRendering) return;
            this.preventRendering = true;
            this.pictureBox.Invoke((MethodInvoker)delegate {
                for (int i = 0; i < this.Bmp.Bits.Length; i++)
                    this.Bmp.Bits[i] = Color.Black.ToArgb();

                float[,] zMax = new float[this.Bmp.Width, this.Bmp.Height];
                for (int i = 0; i < this.Bmp.Width; i++)
                    for (int j = 0; j < this.Bmp.Height; j++)
                    {
                        zMax[i, j] = float.NegativeInfinity;
                    }

                foreach (var subject in this.Subjects)
                {
                    if (subject.needRecalculation || globalChange)
                    {
                        subject.RefreshMatrices(this.MatrixProvider);
                        var transformation = subject.RotationX * subject.Translation * this.viewmodel * this.projection * subject.Viewport;
                        subject.vertices.Clear();
                        for (int i = 0; i < subject.originalVertices.Count; i++)
                        {

                            var screenVertice = Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(subject.originalVertices[i]), transformation));
                            subject.vertices.Add(screenVertice);
                        }
                    }
                    if (globalChange)
                    {
                        Matrix4x4.Invert(Matrix4x4.Transpose(this.viewmodel * this.projection), out subject.NormalTransformation);
                    }

                    for (int j = 0; j < subject.triangles.Count; j++)
                    {
                        var pt1 = subject.vertices[subject.triangles[j].Item1];
                        var pt2 = subject.vertices[subject.triangles[j].Item2];
                        var pt3 = subject.vertices[subject.triangles[j].Item3];

                        var poly = new Polygon();

                        poly.Points.Add(pt1);
                        poly.Points.Add(pt2);
                        poly.Points.Add(pt3);

                        var shouldRender = this.ShadingService.SetShading(subject.triangles[j],
                            light,
                            subject.vertices, subject.normals, subject.NormalTransformation,
                            subject.specularProvider);

                        if (shouldRender)
                        {
                            var colorResolver = new ColorResolver(
                                this.ShadingService, subject.textureProvider);
                            this.FillingService.RunFilling(poly.Points, zMax, colorResolver);
                        }

                    }
                }
                this.globalChange = false;
                this.pictureBox.Invalidate();
                this.preventRendering = false;
            });
        }

        public void ReadVertices(string path)
        {
            string[] data = File.ReadAllLines(path);
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
                    //if ((values[1].Split("/"))[1].Length > 0)
                    //{
                    //    textureIndexes.Add((Int32.Parse((values[1].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[2].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[3].Split("/"))[1], CultureInfo.InvariantCulture.NumberFormat) - 1));
                    //}
                    //else
                    //{
                    //    textureIndexes.Add((Int32.Parse((values[1].Split("/"))[2], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[2].Split("/"))[2], CultureInfo.InvariantCulture.NumberFormat) - 1, Int32.Parse((values[3].Split("/"))[2], CultureInfo.InvariantCulture.NumberFormat) - 1));
                    //}
                }
                //if (data[i][0] == 'v' && data[i][1] == 't')
                //{
                //    if (values.Length == 3)
                //        textureVertices.Add(new Vector2(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat)));
                //    else
                //        textureVertices.Add(new Vector2(float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat)));
                //}
                if (data[i][0] == 'v' && data[i][1] == 'n')
                {
                    if (values.Length == 4)
                        normalForVertices.Add(new Vector3(float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)));
                    else
                        normalForVertices.Add(new Vector3(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat)));
                }
            }
        }
    }
}

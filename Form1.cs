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

        public Vector3 camera = new Vector3(0.0f, 0.0f, 1.0f);

        public Vector3 eye = new Vector3(0.0f, 1.0f, -1.0f);

        public Vector3 center = new Vector3(0.0f, 1, 1.0f);

        public Matrix4x4 projection, viewport, view, model;

        public ITextureProvider BowlingBallProvider { get; set; }

        public ISpecularProvider BowlingBallSpecularProvider { get; set; }

        public List<RenderSubject> Subjects { get; set; }

        public List<List<int>> verticeInTriangles = new List<List<int>>();

        public int frameCounter = 20;

        public bool preventRendering = false;

        public List<Vector3> LightVectors = new List<Vector3>();

        public List<CameraSubject> cameras = new List<CameraSubject>();

        public CameraSubject selectedCamera { get; set; }

        public CameraSubject awaitingCamera = null;

        public Form1()
        {
            InitializeComponent();

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

            this.ShadingService.Type = ShadingType.Phong;
            
            this.BowlingBallProvider = new BowlingPinProvider();
            this.BowlingBallSpecularProvider = new BowlingBallSpecularProvider();

            this.projection = this.MatrixProvider.ProjectionMatrix(camera);
            this.view = this.MatrixProvider.ViewMatrix(eye, center, new Vector3(0, 1, 0));
            this.model = this.MatrixProvider.ModelMatrix(eye, center, new Vector3(0, 1, 0));

            this.Subjects = new List<RenderSubject>();

            var pinPath = "../../../pin.obj";
            var ballPath = "../../../BowlingBall2.obj";

            ReadVertices(pinPath);
            for (int i = 0; i < 3; i++)
            {
                var pin = new RenderSubject();
                pin.size = 800;
                pin.posY = 400;
                pin.posX = 450;

                pin.baseAngle = 0;
                pin.diffAngle = 0;
                pin.baseTranslationPosition = new Vector3(0.37f - i * 0.3f, 0, 2.5f);
                pin.translationPosition = pin.baseTranslationPosition;
                //pin.diffPosition = new Vector3(0.05f, 0, 0);
                pin.frameRestart = 60;
                pin.RefreshMatrices(MatrixProvider);

                pin.textureProvider = new BowlingPinProvider();
                pin.specularProvider = new BowlingBallSpecularProvider();

                pin.originalVertices.AddRange(this.originalVertices);
                pin.triangles.AddRange(this.triangles);
                pin.normals.AddRange(this.normalForVertices);
                //pin.normals = pin.normals.Select(v => new Vector3(-v.X, -v.Y, -v.Z)).ToList();
                pin.NormalAdjusting = Matrix4x4.Identity;
                pin.NormalAdjusting.M11 = -1;
                pin.NormalAdjusting.M22 = -1;
                pin.NormalAdjusting.M33 = -1;
                pin.InitMatrices(MatrixProvider);
                this.Subjects.Add(pin);
            }

            
            this.originalVertices.Clear();
            this.triangles.Clear();
            this.normalForVertices.Clear();
            this.verticeInTriangles.Clear();
            ReadVertices(ballPath);

            var ball = new RenderSubject();
            ball.size = 800;
            ball.posX = 450;
            ball.posY = 420;

            ball.baseAngle = 0;
            ball.angle = 0;
            ball.diffAngle = 0.5f;
            ball.baseTranslationPosition = new Vector3(0.0f, 0, 1.2f);
            ball.translationPosition = new Vector3(0.0f, 0, 1.2f);
            ball.diffPosition = new Vector3(0.0f, 0.0f, 0.05f);

            ball.frameRestart = 30;
            ball.specularProvider = new BowlingBallSpecularProvider();
            ball.textureProvider = new BowlingBallProvider();

            ball.originalVertices.AddRange(this.originalVertices);
            ball.triangles.AddRange(this.triangles);           
            ball.normals.AddRange(this.normalForVertices);
            ball.NormalAdjusting = Matrix4x4.Identity;

            ball.NormalAdjusting.M11 = 1;
            ball.NormalAdjusting.M22 = 1;
            ball.NormalAdjusting.M33 = -1;
            
            ball.InitMatrices(MatrixProvider);
            this.Subjects.Add(ball);

            var constCam = new CameraSubject(this, CameraType.Constant);
            constCam.SetConstCamera(new Vector3(0, 0, 0.2f));
            this.cameras.Add(constCam);

            var lookingCam = new CameraSubject(this, CameraType.Looking);
            lookingCam.LookSubject(ball, new Vector3(1, 0, 0));
            this.cameras.Add(lookingCam);

            var movingCam = new CameraSubject(this, CameraType.Following);
            movingCam.FollowSubject(ball, ball.diffPosition);
            this.cameras.Add(movingCam);

            this.selectedCamera = constCam;


            this.InitLights();

            this.view = this.MatrixProvider.ViewMatrix(eye, center, new Vector3(0, 1, 0));
            this.model = this.MatrixProvider.ModelMatrix(eye, center, new Vector3(0, 1, 0));
          }

        public void InitLights()
        {
            this.LightVectors.Add(new Vector3(1.0f, 1.0f, 1.0f));
        }

        public void RefeshLights()
        {
            this.LightVectors = this.LightVectors.Select(l => Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(Vector3.Normalize(l)), this.view * this.model))).ToList();
            this.LightVectors[0] = new Vector3(this.LightVectors[0].X, this.LightVectors[0].Y + 0.05f, this.LightVectors[0].Z);
        }

        public void Render(object state)
        {
            if (this.preventRendering) return;
            this.preventRendering = true;
            this.pictureBox.Invoke((MethodInvoker)delegate {
                for (int i = 0; i < this.Bmp.Bits.Length; i++)
                    this.Bmp.Bits[i] = Color.Black.ToArgb();

                this.RefeshLights();

                if (awaitingCamera != null)
                {
                    this.selectedCamera = awaitingCamera;
                    this.awaitingCamera = null;
                }

                foreach (var subject in this.Subjects)
                {
                    subject.RefreshMatrices(this.MatrixProvider);
                }

                this.selectedCamera.Refresh();
                this.view = MatrixProvider.ViewMatrix(eye, center, new Vector3(0, 1, 0));
                this.model = MatrixProvider.ModelMatrix(eye, center, new Vector3(0, 1, 0));

                foreach (var subject in this.Subjects)
                {
                    var transformation = subject.RotationX * subject.Translation * this.view 
                        * this.model * this.selectedCamera.projectionTransformation * this.projection * subject.Viewport;
                    subject.vertices.Clear();
                    for (int i = 0; i < subject.originalVertices.Count; i++)
                    {
                        var screenVertice = Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(subject.originalVertices[i]), transformation));
                        subject.vertices.Add(screenVertice);
                    }
                    Matrix4x4.Invert(Matrix4x4.Transpose(
                        subject.RotationX * this.view * this.model * this.selectedCamera.projectionTransformation * this.projection),
                        out subject.NormalTransformation);
                    subject.NormalTransformation *= subject.NormalAdjusting;
                    float[,] zMax = new float[this.Bmp.Width, this.Bmp.Height];
                    for (int i = 0; i < this.Bmp.Width; i++)
                        for (int j = 0; j < this.Bmp.Height; j++)
                        {
                            zMax[i, j] = float.NegativeInfinity;
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
                            this.LightVectors,
                            subject.originalVertices, subject.normals, subject.NormalTransformation,
                            subject.specularProvider);

                        if (shouldRender)
                        {
                            var colorResolver = new ColorResolver(
                                this.ShadingService, subject.textureProvider);
                            this.FillingService.RunFilling(poly.Points, zMax, colorResolver);
                        }

                    }
                }
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
                        originalVertices.Add(new Vector3(float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)));
                    else
                        originalVertices.Add(new Vector3(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                         float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                         float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat)));

                    verticeInTriangles.Add(new List<int>());

                }
                if (data[i][0] == 'f')
                {
                    var idx1 = Int32.Parse((values[1].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1;
                    var idx2 = Int32.Parse((values[2].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1;
                    var idx3 = Int32.Parse((values[3].Split("/"))[0], CultureInfo.InvariantCulture.NumberFormat) - 1;
                    triangles.Add((idx1, idx2, idx3));

                    verticeInTriangles[idx1].Add(triangles.Count - 1);
                    verticeInTriangles[idx2].Add(triangles.Count - 1);
                    verticeInTriangles[idx3].Add(triangles.Count - 1);
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
            // this means that obj file does not provide correct normals (pretty common situation) and they need calculation
            //if (this.normalForVertices.Count < this.originalVertices.Count)
            //{
                this.normalForVertices.Clear();
                //Matrix4x4.Invert(Matrix4x4.Transpose(this.viewmodel * this.projection), out var normalTransformation);
                //for (int i = 0; i < triangles.Count; i++)
                //{
                //    //var neighbours = Helpers.findNeighbours(triangles, triangles[i]);
                //    var normals = new List<Vector3>();
                //    var centers = new List<Vector3>();
                //    foreach (var triangle in neighbours)
                //    {
                //        Vector3 v1 = new Vector3(originalVertices[triangle.Item2].X - originalVertices[triangle.Item1].X,
                //           originalVertices[triangle.Item2].Y - originalVertices[triangle.Item1].Y,
                //           originalVertices[triangle.Item2].Z - originalVertices[triangle.Item1].Z);
                //        Vector3 v2 = new Vector3(originalVertices[triangle.Item3].X - originalVertices[triangle.Item1].X,
                //            originalVertices[triangle.Item3].Y - originalVertices[triangle.Item1].Y,
                //            originalVertices[triangle.Item3].Z - originalVertices[triangle.Item1].Z);
                //        normals.Add(Vector3.Normalize(Vector3.Cross(v2, v1)));
                //        centers.Add(Helpers.MidPoint(triangle, originalVertices));
                //    }
                //    normalForVertices.Add((normals[0] + normals[1] + normals[2]) / 3);
                //    //var midPoint = Helpers.MidPoint(triangles[i], originalVertices);
                //    //var bayCoords = Helpers.GetBaycentricCoords(centers, new Vector2(midPoint.X, midPoint.Y));
                //    //normalForVertices.Add(bayCoords.X * normals[0] + bayCoords.Y * normals[1] + bayCoords.Z * normals[2]);
                //}
                for (int i = 0; i < originalVertices.Count; i++)
                {
                    var neighbourTriangles = new List<(int, int, int)>();
                    for (int j = 0; j < verticeInTriangles[i].Count; j++)
                    {
                        neighbourTriangles.Add(triangles[verticeInTriangles[i][j]]);
                    }
                    var normals = new List<Vector3>();
                    foreach (var triangle in neighbourTriangles)
                    {
                        Vector3 v1 = originalVertices[triangle.Item2] - originalVertices[triangle.Item1];
                        Vector3 v2= originalVertices[triangle.Item3] - originalVertices[triangle.Item1];
                        normals.Add(Vector3.Normalize(Vector3.Cross(v1, v2)));
                    }
                    var vecSum = new Vector3(0, 0, 0);
                    foreach (var normal in normals)
                        vecSum += normal;
                    if (normals.Count > 0)
                        vecSum /= normals.Count;
                    normalForVertices.Add(vecSum);
                }
            //}
        }

        private void constBtn_Click(object sender, EventArgs e)
        {
            this.ShadingService.Type = ShadingType.Constant;
        }

        private void gouraudBtn_Click(object sender, EventArgs e)
        {
            this.ShadingService.Type = ShadingType.Gouraud;
        }

        private void phongBtn_Click(object sender, EventArgs e)
        {
            this.ShadingService.Type = ShadingType.Phong;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.timer = new System.Threading.Timer(this.Render, null, 0, 1000 / frameCounter);

        }

        private void cam1Btn_Click(object sender, EventArgs e)
        {
            this.awaitingCamera = this.cameras[0];
        }

        private void cam2Btn_Click(object sender, EventArgs e)
        {
            this.awaitingCamera = this.cameras[1];
        }

        private void cam3Btn_Click(object sender, EventArgs e)
        {
            this.awaitingCamera = this.cameras[2];
        }

    }
}

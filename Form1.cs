using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MysteryProject
{
    public partial class Form1 : Form
    {
        public BresenhamLine BresenhamLine { get; set; }

        public DirectBitmap Bmp { get; set; }

        public Form1()
        {
            InitializeComponent();

            this.Bmp = new DirectBitmap(this.pictureBox.Width, this.pictureBox.Height);

            this.pictureBox.Image = this.Bmp.Bitmap;

            this.BresenhamLine = new BresenhamLine(this.Bmp);

            //Matrix<double> a = DenseMatrix.OfArray(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
            //Matrix<double> b = DenseMatrix.OfArray(new double[,] { { 1 }, { 0 }, { 0 } });
            //Matrix<double> result = a * b;
            //var es = DenseMatrix.CreateIdentity(10);
        }
    }
}

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class MatrixProvider
    {

        public Matrix4x4 ProjectionMatrix(Vector3 camera)
        {
            Matrix4x4 projection = Matrix4x4.Identity;
            projection.M34 = -1 / camera.Z;
            return projection;
        }

        public Matrix4x4 ViewportMatrix(int positionX, int positionY, int size)
        {
            Matrix4x4 viewport = Matrix4x4.Identity;
            viewport.M11 = size / 2;
            viewport.M22 = size / 2;
            viewport.M33 = 255 / 2;

            viewport.M41 = positionY;
            viewport.M42 = positionX;
            viewport.M43 = 255 / 2;

            return viewport;
        }

        public Matrix4x4 ViewModelMatrix(Vector3 eye, Vector3 center, Vector3 up)
        {
            Matrix4x4 viewModel = Matrix4x4.Identity;
            Vector3 z = Vector3.Normalize(eye - center);
            Vector3 x = Vector3.Normalize(Vector3.Cross(up, z));
            Vector3 y = Vector3.Normalize(Vector3.Cross(z, x));

            viewModel.M11 = x.X;
            viewModel.M21 = x.Y;
            viewModel.M31 = x.Z;
            
            viewModel.M12 = y.X;
            viewModel.M22 = y.Y;
            viewModel.M32 = y.Z;

            viewModel.M13 = z.X;
            viewModel.M23 = z.Y;
            viewModel.M33 = z.Z;

            viewModel.M41 = -center.X;
            viewModel.M42 = -center.Y;
            viewModel.M43 = -center.Z;

            return viewModel;
        }

        public Matrix4x4 RotationMatrixX(float angle)
        {
            var matrix = Matrix4x4.Identity;
            matrix.M22 = (float)Math.Cos(angle);
            matrix.M32 = -(float)Math.Sin(angle);
            matrix.M33 = (float)Math.Cos(angle);
            matrix.M23 = (float)Math.Sin(angle);
            return matrix;
        }

        public Matrix4x4 RotationMatrixY(float angle)
        {
            var matrix = Matrix4x4.Identity;
            matrix.M11 = (float)Math.Cos(angle);
            matrix.M13 = -(float)Math.Sin(angle);
            matrix.M33 = (float)Math.Cos(angle);
            matrix.M31 = (float)Math.Sin(angle);
            return matrix;
        }

        public Matrix4x4 RotationMatrixZ(float angle)
        {
            var matrix = Matrix4x4.Identity;
            matrix.M11 = (float)Math.Cos(angle);
            matrix.M21 = -(float)Math.Sin(angle);
            matrix.M22 = (float)Math.Cos(angle);
            matrix.M12 = (float)Math.Sin(angle);
            return matrix;
        }

        public Matrix4x4 TranslationMatrix(Vector3 translation)
        {
            var matrix = Matrix4x4.Identity;
            matrix.M41 = translation.X;
            matrix.M42 = translation.Y;
            matrix.M43 = translation.Z;
            return matrix;
        }
    }
}

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

        public Matrix4x4 ViewportnMatrix(int positionX, int positionY, int size)
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
    }
}

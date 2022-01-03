using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class RenderSubject
    {
        public int posX;
        public int posY;
        public int size;

        public float baseAngle;
        public float angle;
        public float diffAngle;

        public Vector3 baseTranslationPosition;
        public Vector3 translationPosition;
        public Vector3 diffPosition;

        private int counter;
        public int frameRestart;

        public Matrix4x4 Viewport;
        public Matrix4x4 RotationX;
        public Matrix4x4 Translation;

        public Matrix4x4 NormalTransformation;

        public List<(int, int, int)> triangles = new List<(int, int, int)>();
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector3> originalVertices = new List<Vector3>();

        public bool needRecalculation;

        public ITextureProvider textureProvider;
        public ISpecularProvider specularProvider;

        public void InitMatrices(MatrixProvider matrixProvider)
        {
            this.Viewport = matrixProvider.ViewportMatrix(posX, posY, size);
        }

        public void RefreshMatrices(MatrixProvider matrixProvider)
        {
            this.angle += diffAngle;
            this.translationPosition += diffPosition;
            counter++;
            if (counter > frameRestart)
            {
                counter = 0;
                this.angle = baseAngle;
                this.translationPosition = baseTranslationPosition;
            }

            this.RotationX = matrixProvider.RotationMatrixY(angle);
            this.Translation = matrixProvider.TranslationMatrix(translationPosition);
        }
    }
}

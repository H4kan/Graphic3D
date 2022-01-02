using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class ShadingService
    {
        public ShadingType Type { get; set; }

        public DirectBitmap SpecularBmp { get; set; }

        private float intensity;

        private (float, float, float) gouruadIntensities;

        private (Vector3, Vector3, Vector3) phongNormals;

        private Vector3 phongLightVector;

        private Matrix4x4 phongNormalTransformation;
        

        public bool SetShading((int, int, int) triangle, Vector3 lightVector, List<Vector3> vertices, List<Vector3> normalForVertices, Matrix4x4 normalTransformation)
        {
            switch (Type)
            {
                case ShadingType.Constant:
                    return SetConstantShading(vertices, triangle, lightVector, normalTransformation);
                case ShadingType.Gouraud:
                    return SetGouraudShading(normalForVertices, triangle, lightVector, normalTransformation);
                case ShadingType.Phong:
                    return SetPhongShading(normalForVertices, triangle, lightVector, normalTransformation);
            }
            return true;
        }

        public bool SetConstantShading(List<Vector3> vertices, (int, int, int) triangle, Vector3 lightVector, Matrix4x4 normalTransformation)
        {
            Vector3 v1 = new Vector3(vertices[triangle.Item2].X - vertices[triangle.Item1].X,
                    vertices[triangle.Item2].Y - vertices[triangle.Item1].Y,
                    vertices[triangle.Item2].Z - vertices[triangle.Item1].Z);
            Vector3 v2 = new Vector3(vertices[triangle.Item3].X - vertices[triangle.Item1].X,
                vertices[triangle.Item3].Y - vertices[triangle.Item1].Y,
                vertices[triangle.Item3].Z - vertices[triangle.Item1].Z);

            var normalVec = Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(Vector3.Cross(v2, v1)), normalTransformation)));

            this.intensity = Vector3.Dot(normalVec, lightVector);

            return this.intensity > 0;
        }

        public bool SetGouraudShading(List<Vector3> normalForVertices, (int, int, int) triangle, Vector3 lightVector, Matrix4x4 normalTransformation)
        {
           if (triangle.Item1 < normalForVertices.Count && triangle.Item2 < normalForVertices.Count && triangle.Item3 < normalForVertices.Count)
            this.gouruadIntensities = (
                Vector3.Dot(Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(normalForVertices[triangle.Item1]), normalTransformation))), lightVector),
                Vector3.Dot(Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(normalForVertices[triangle.Item2]), normalTransformation))), lightVector),
               Vector3.Dot(Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(normalForVertices[triangle.Item3]), normalTransformation))), lightVector));
            return this.gouruadIntensities.Item1 > 0 || this.gouruadIntensities.Item2 > 0 || this.gouruadIntensities.Item3 > 0;
        }

        public bool SetPhongShading(List<Vector3> normalForVertices, (int, int, int) triangle, Vector3 lightVector, Matrix4x4 normalTransformation)
        {
            this.phongNormals = (normalForVertices[triangle.Item1], normalForVertices[triangle.Item2], normalForVertices[triangle.Item3]);
            this.phongLightVector = lightVector;
            this.phongNormalTransformation = normalTransformation;
            return this.phongNormals.Item1 != Vector3.Zero || this.phongNormals.Item2 != Vector3.Zero || this.phongNormals.Item3 != Vector3.Zero;
        }

        public float GetShading(Vector3 bayCoords, Vector3 point, out float spec)
        {
            spec = 0;
            switch (Type)
            {
                case ShadingType.Constant:
                    return intensity;
                case ShadingType.Gouraud:
                    return EvaluateGouraudShading(bayCoords);
                case ShadingType.Phong:
                    return EvaluatePhongShading(bayCoords, point, out spec);
            }
            return 0;
        }
        
        public float EvaluatePhongShading(Vector3 bayCoords, Vector3 point, out float spec)
        {
            Vector3 weightedNormal = bayCoords.X * this.phongNormals.Item1 + bayCoords.Y * this.phongNormals.Item2 + bayCoords.Z * this.phongNormals.Item3;

            var transformedNormal = Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(weightedNormal), this.phongNormalTransformation)));

            spec = 0;
            if (this.SpecularBmp != null)
            {
                Vector3 reflected = Vector3.Normalize(transformedNormal * Vector3.Dot(transformedNormal, this.phongLightVector) * 2 - this.phongLightVector);

                spec = (float)Math.Pow(Math.Max(0, reflected.Z), this.SpecularBmp.GetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)).R);
            }

            return Vector3.Dot(transformedNormal, this.phongLightVector);
        }

        public float EvaluateGouraudShading(Vector3 bayCoords)
        {
            return bayCoords.X * gouruadIntensities.Item1 + bayCoords.Y * gouruadIntensities.Item2 + bayCoords.Z * gouruadIntensities.Item3;
        }
    }
}

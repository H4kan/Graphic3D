using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private List<Vector3> phongLightVectors;

        private Matrix4x4 phongNormalTransformation;

        private ISpecularProvider specularProvider;
        

        public bool SetShading((int, int, int) triangle, List<Vector3> lightVectors, List<Vector3> vertices, List<Vector3> normalForVertices, Matrix4x4 normalTransformation, ISpecularProvider specularProvider = null)
        {
            switch (Type)
            {
                case ShadingType.Constant:
                    return SetConstantShading(vertices, triangle, lightVectors, normalTransformation);
                case ShadingType.Gouraud:
                    return SetGouraudShading(normalForVertices, triangle, lightVectors, normalTransformation);
                case ShadingType.Phong:
                    return SetPhongShading(normalForVertices, triangle, lightVectors, normalTransformation, specularProvider);
            }
            return true;
        }

        public bool SetConstantShading(List<Vector3> vertices, (int, int, int) triangle, List<Vector3> lightVectors, Matrix4x4 normalTransformation)
        {
            Vector3 v1 = vertices[triangle.Item2] - vertices[triangle.Item1];
            Vector3 v2 = vertices[triangle.Item3] - vertices[triangle.Item1];

            var normalVec = Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(Vector3.Normalize(Vector3.Cross(v1, v2))), normalTransformation));

            this.intensity = 0;
            foreach (var lightVec in lightVectors)
                this.intensity += Math.Max(0, Vector3.Dot(normalVec, lightVec));
            return this.intensity > 0;
        }

        public bool SetGouraudShading(List<Vector3> normalForVertices, (int, int, int) triangle, List<Vector3> lightVectors, Matrix4x4 normalTransformation)
        {
       
            var normal1 = Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(normalForVertices[triangle.Item1]), normalTransformation)));
            var normal2 = Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(normalForVertices[triangle.Item2]), normalTransformation)));
            var normal3 = Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(normalForVertices[triangle.Item3]), normalTransformation)));

            float intensity1 = 0.0f, intensity2 = 0.0f, intensity3 = 0.0f;
            for (int i = 0; i < lightVectors.Count; i++)
            {
                intensity1 += Math.Max(0, Vector3.Dot(normal1, lightVectors[i]));
                intensity2 += Math.Max(0, Vector3.Dot(normal2, lightVectors[i]));
                intensity3 += Math.Max(0, Vector3.Dot(normal3, lightVectors[i]));
            }
            this.gouruadIntensities = (intensity1, intensity2, intensity3);

             return this.gouruadIntensities.Item1 > 0 || this.gouruadIntensities.Item2 > 0 || this.gouruadIntensities.Item3 > 0;
        }

        public bool SetPhongShading(List<Vector3> normalForVertices, (int, int, int) triangle, List<Vector3> lightVectors, Matrix4x4 normalTransformation, ISpecularProvider specularProvider)
        {
            this.phongNormals = (normalForVertices[triangle.Item1], normalForVertices[triangle.Item2], normalForVertices[triangle.Item3]);
            this.phongLightVectors = lightVectors;
            this.phongNormalTransformation = normalTransformation;
            this.specularProvider = specularProvider;
            return this.phongNormals.Item1 != Vector3.Zero || this.phongNormals.Item2 != Vector3.Zero || this.phongNormals.Item3 != Vector3.Zero;
        }

        // specular bmp works only with phong since it decreases performance
        public float GetShading(Vector3 bayCoords, Vector3 point, out bool spec)
        {
            spec = false;
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
        
        public float EvaluatePhongShading(Vector3 bayCoords, Vector3 point, out bool spec)
        {
            Vector3 weightedNormal = bayCoords.X * this.phongNormals.Item1 + bayCoords.Y * this.phongNormals.Item2 + bayCoords.Z * this.phongNormals.Item3;

            var transformedNormal = Vector3.Normalize(Helpers.Vector4ToVector3(Vector4.Transform(Helpers.Vector3ToVector4(weightedNormal), this.phongNormalTransformation)));

            float intensity = 0;
            if (this.specularProvider != null)
            {
                spec = true;
                for (int i = 0; i < this.phongLightVectors.Count; i++)
                {
                    Vector3 reflected = Vector3.Normalize(transformedNormal * Vector3.Dot(transformedNormal, this.phongLightVectors[i]) * 2 - this.phongLightVectors[i]);
                    float specularValue = this.specularProvider.ResolveSpecular(reflected);
                    intensity += Math.Max(0, Vector3.Dot(transformedNormal, this.phongLightVectors[i])) + 1.3f * specularValue;
                    if (float.IsNaN(intensity))
                        intensity = float.MaxValue;
                }
            }
            else
            {
                spec = false;
                for (int i = 0; i < this.phongLightVectors.Count; i++)
                {
                    intensity += Math.Max(0, Vector3.Dot(transformedNormal, this.phongLightVectors[i]));
                }
            }

            return intensity;
        }

        public float EvaluateGouraudShading(Vector3 bayCoords)
        {
            return bayCoords.X * gouruadIntensities.Item1 + bayCoords.Y * gouruadIntensities.Item2 + bayCoords.Z * gouruadIntensities.Item3;
        }
    }
}

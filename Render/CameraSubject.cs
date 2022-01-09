using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public enum CameraType
    {
        Constant = 0,
        Following,
        Looking
    };
    public class CameraSubject
    {
        public CameraType type { get; set; }

        public Form1 form;

        public RenderSubject renderSubject { get; set; }

        public Vector3 offsetPosition { get; set; }

        public Vector3 translPosition { get; set; }
         
        public Matrix4x4 translProjection { get; set; }

        public Matrix4x4 projectionTransformation { get; set; }


        public CameraSubject(Form1 form, CameraType type)
        {
            this.type = type;
            this.form = form;
        }

        public void FollowSubject(RenderSubject subj, Vector3 offsetPosition)
        {
            renderSubject = subj;
            this.translPosition = new Vector3(0, 0, 0);
            this.offsetPosition = offsetPosition;
            //this.form.center = subj.baseTranslationPosition + offsetPosition;
        }

        public void LookSubject(RenderSubject subj, Vector3 position)
        {
            renderSubject = subj;
            this.translPosition = position;
            this.translProjection = this.form.MatrixProvider.TranslationMatrix(position);
        }

        public void SetConstCamera(Vector3 position)
        {
            Matrix4x4.Invert(this.form.MatrixProvider.TranslationMatrix(position), out var invertedTransl);
            this.projectionTransformation = invertedTransl;
        }

        public void Refresh()
        {
            switch (this.type)
            {
                case CameraType.Following:
                    //this.form.center = renderSubject.translationPosition + offsetPostion;
                    this.translPosition = this.renderSubject.baseTranslationPosition 
                        - this.renderSubject.translationPosition;
                    this.projectionTransformation = this.form.MatrixProvider.TranslationMatrix(this.translPosition);
                    break;
                case CameraType.Looking:
                    var diffVec =  this.translPosition - this.renderSubject.translationPosition;
                    var followingAngle = Math.PI / 2 - Math.Acos(1 / diffVec.Length());

                    Matrix4x4.Invert(
                        this.form.MatrixProvider.RotationMatrixY((float)(followingAngle)) * 
                        this.translProjection, out var invertedLookMat);
                    this.projectionTransformation = invertedLookMat;
                    break;
                case CameraType.Constant:
                    break;

            }
        }
    }
}

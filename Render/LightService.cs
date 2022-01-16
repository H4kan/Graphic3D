using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MysteryProject
{
    public class LightService
    {
        private Form1 form;

        public Matrix4x4 invertCameraPosition;

        private List<LightSubject> lights = new List<LightSubject>();

        public LightService(Form1 form)
        {
            this.form = form;
        }

        public void AddLight(LightSubject subj)
        {
            this.lights.Add(subj);
        }

        public void UpdateLights()
        {
            lights.ForEach(l => l.UpdateLight());
        }

        public List<Vector3> ResolveLightVectors()
        {
            return lights.Select(l =>
                Vector3.Transform(l.currentPosition, invertCameraPosition)
            ).ToList();
        }
    }

    public class LightSubject
    {
        public Vector3 currentPosition { get; set; }

        public LightSubject(Vector3 initPosition)
        {
            this.currentPosition = initPosition;
        }

        public virtual void UpdateLight() {}
    }

    public class FollowingLightSubject : LightSubject
    {
        public RenderSubject renderSubject { get; set; }

        public Vector3 initPosition;

        public FollowingLightSubject(Vector3 initPosition, RenderSubject subject) : base(initPosition)
        {
            this.initPosition = initPosition;
            this.renderSubject = subject;
        }

        public override void UpdateLight()
        {
            this.currentPosition = Vector3.Transform(this.initPosition, renderSubject.Translation);
            base.UpdateLight();
        }
    }
}

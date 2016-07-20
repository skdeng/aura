using System;
using System.Numerics;

namespace Aura
{
    class Camera
    {
        public Vector3 Position { get; set; }

        Vector3 _Direction;
        public Vector3 Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value.Copy();
                if (Up == null)
                {
                    return;
                }
                UpdateReferenceFrame();
            }
        }

        Vector3 _Up;
        public Vector3 Up
        {
            get { return _Up; }
            set
            {
                _Up = value.Copy();
                if (Direction == null)
                {
                    return;
                }
                UpdateReferenceFrame();
            }
        }

        public double FOVY { get; set; }

        private double FOVX { get { return FOVY * ImageWidth / ImageHeight; } }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        private Vector3 U { get; set; }

        private Vector3 V { get; set; }

        private Vector3 W { get; set; }

        public Camera()
        {

        }

        public Camera(Scene sceneDescription)
        {
            Position = sceneDescription.CameraPosition;
            Direction = sceneDescription.CameraDirection;
            Up = sceneDescription.CameraUp;
            FOVY = sceneDescription.CameraFOV;
            ImageWidth = sceneDescription.ImageWidth;
            ImageHeight = sceneDescription.ImageHeight;
        }

        public Ray GetRay(Sample sample)
        {
            // optimize outside
            var thetaX = (float)Math.Tan((Math.PI / 180) * FOVX / 2);
            var thetaY = (float)Math.Tan((Math.PI / 180) * FOVY / 2);

            var halfWidth = ImageWidth / 2;
            var halfHeight = ImageHeight / 2;

            var alpha = thetaX * ((sample.X + sample.OffsetX) - halfWidth) / halfWidth;
            var beta = thetaY * (halfHeight - (sample.Y - sample.OffsetY)) / halfHeight;

            return new Ray(Position, Vector3.Normalize(alpha * U + beta * V - W));
        }

        private void UpdateReferenceFrame()
        {
            W = -Vector3.Normalize(_Direction);
            U = Vector3.Normalize(Vector3.Cross(Up, W));
            V = Vector3.Normalize(Vector3.Cross(W, U));
        }
    }
}

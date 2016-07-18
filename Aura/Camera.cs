using Aura.VecMath;
using System;

namespace Aura
{
    class Camera
    {
        public Vec3 Position { get; set; }

        Vec3 _Direction;
        public Vec3 Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value.Clone() as Vec3;
                if (Up == null)
                {
                    return;
                }
                W = -_Direction.Normalize();
                U = Up.Cross(W).Normalize();
                V = W.Cross(U).Normalize();
            }
        }

        Vec3 _Up;
        public Vec3 Up
        {
            get { return _Up; }
            set
            {
                _Up = value.Clone() as Vec3;
                if (Direction == null)
                {
                    return;
                }
                W = -Direction.Normalize();
                U = _Up.Cross(W).Normalize();
                V = W.Cross(U).Normalize();
            }
        }

        public double FOVY { get; set; }

        private double FOVX { get { return FOVY * ImageWidth / ImageHeight; } }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        private Vec3 U { get; set; }

        private Vec3 V { get; set; }

        private Vec3 W { get; set; }

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
            var thetaX = Math.Tan((Math.PI / 180) * FOVX / 2);
            var thetaY = Math.Tan((Math.PI / 180) * FOVY / 2);

            var halfWidth = ImageWidth / 2;
            var halfHeight = ImageHeight / 2;

            var alpha = thetaX * ((sample.X + sample.OffsetX) - halfWidth) / halfWidth;
            var beta = thetaY * (halfHeight - (sample.Y - sample.OffsetY)) / halfHeight;

            return new Ray(Position, (alpha * U + beta * V - W).Normalize());
        }
    }
}

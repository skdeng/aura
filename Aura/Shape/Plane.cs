using Aura.Values;
using System;
using System.Numerics;

namespace Aura.Shape
{
    class Plane : Primitive
    {
        public Vector3 Normal { get; set; }
        public Vector3 Origin { get; set; }
        public Material SurfaceMaterialSecondary { get; set; }

        public override Matrix4x4 Transform
        {
            get
            {
                return base.Transform;
            }

            set
            {
                base.Transform = value;
                if (!Transform.IsIdentity)
                {
                    Origin = Vector3.Transform(Origin, Transform);
                    Normal = Vector3.TransformNormal(Normal, Transform);
                }
            }
        }

        public override Intersection Intersect(Ray ray)
        {
            var rayPlaneAngle = Vector3.Dot(ray.Direction, Normal);

            if (rayPlaneAngle > -Constant.PlaneHorizon && rayPlaneAngle < Constant.PlaneHorizon)
            {
                return null;
            }
            else
            {
                var tempT = Vector3.Dot((Origin - ray.Position), Normal) / rayPlaneAngle;

                if (tempT < 0)
                {
                    return null;
                }

                var intersectionPoint = ray + tempT;

                return new Intersection()
                {
                    T = tempT,
                    ContactObject = this,
                    Normal = Normal,
                    Position = intersectionPoint,
                    ContactMaterial = (Material)(((Math.Floor(intersectionPoint.X) + Math.Floor(intersectionPoint.Z)) % 2 == 0) ? SurfaceMaterial : SurfaceMaterialSecondary ?? SurfaceMaterial).Clone()
                };
            }
        }
    }
}

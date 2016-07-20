using Aura.Values;
using Aura.VecMath;
using System;

namespace Aura.Shape
{
    class Plane : Primitive
    {
        public Vec3 Normal { get; set; }
        public Vec3 Origin { get; set; }
        public Material SurfaceMaterialSecondary { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            if (Transform != null)
            {
                var transformedRay = new Ray(Transform.TransformPoint(ray.Position), Transform.TransformVector(ray.Direction));
            }

            double rayPlaneAngle = ray.Direction.Dot(Normal);

            if (rayPlaneAngle > -Constant.PlaneHorizon && rayPlaneAngle < Constant.PlaneHorizon)
            {
                return new Intersection() { Intersect = false };
            }
            else
            {
                double tempT = (Origin - ray.Position).Dot(Normal) / rayPlaneAngle;

                if (tempT < ray.Min || tempT > ray.Max)
                {
                    return new Intersection() { Intersect = false };
                }

                var intersectionPoint = ray + tempT;

                return new Intersection()
                {
                    Intersect = true,
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

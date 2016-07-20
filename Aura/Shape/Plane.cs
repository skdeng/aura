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

        public override Intersection Intersect(Ray ray)
        {
            Ray transformedRay = TransformRay(ray);
            
            var rayPlaneAngle = Vector3.Dot(transformedRay.Direction, Normal);

            if (rayPlaneAngle > -Constant.PlaneHorizon && rayPlaneAngle < Constant.PlaneHorizon)
            {
                return null;
            }
            else
            {
                var tempT = Vector3.Dot((Origin - transformedRay.Position), Normal) / rayPlaneAngle;

                if (tempT < 0)
                {
                    return null;
                }

                var intersectionPoint = transformedRay + tempT;

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

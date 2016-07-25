using Aura.Values;
using System;
using System.Numerics;

namespace Aura.Shape
{
    class Sphere : Primitive
    {
        public Vector3 Center { get; set; }

        private float _Radius { get; set; }
        public float Radius
        {
            get
            {
                return _Radius;
            }
            set
            {
                _Radius = value;
                RadiusSq = _Radius * _Radius;
            }
        }

        private float RadiusSq { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            Vector3 oc = Center - ray.Position;
            var ocLengthSq = oc.LengthSquared();
            var directionDotOC = Vector3.Dot(ray.Direction, oc);
            var determinant = directionDotOC * directionDotOC - ocLengthSq + RadiusSq;

            // No intersection
            if (determinant < -Constant.Epsilon)
            {
                return null;
            }

            // Determinant larger than zero => 2 intersections
            bool inside = false;
            float tempT = 0;
            if (determinant > Constant.Epsilon)
            {
                determinant = (float)Math.Sqrt(determinant);

                // Find closest intersection
                if (directionDotOC - determinant > 0)
                {
                    tempT = directionDotOC - determinant;
                }
                else if (directionDotOC + determinant > 0)
                {
                    tempT = directionDotOC + determinant;
                    inside = true;
                }
                else
                {
                    return null;
                }
            }
            else // tangent => no intersection
            {
                return null;
            }

            var intersection = new Intersection() { T = tempT, ContactObject = this, ContactMaterial = (Material)SurfaceMaterial.Clone(), Position = ray + tempT, Inside = inside };
            intersection.Normal = Vector3.Normalize(intersection.Position - Center);

            return intersection;
        }
    }
}

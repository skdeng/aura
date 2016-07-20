using System;
using System.Numerics;

namespace Aura.Shape
{
    class Sphere : Primitive
    {
        public Vector3 Center { get; set; }

        private double _Radius { get; set; }
        public double Radius
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

        private double RadiusSq { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            Vector3 oc = ray.Position - Center;
            var ocLength = oc.Length();
            var directionDotOC = Vector3.Dot(ray.Direction, oc);
            var determinant = directionDotOC * directionDotOC - ocLength * ocLength + RadiusSq;

            // No intersection
            if (determinant < 0 || directionDotOC > 0)
            {
                return null;
            }

            var tempT = -directionDotOC;
            
            // Determinant larger than zero => 2 intersections
            bool inside = false;
            if (determinant > 0)
            {
                var sqrtDeterminant = (float) Math.Sqrt(determinant);

                // Find closest intersection
                if (sqrtDeterminant > tempT)
                {
                    tempT += sqrtDeterminant;
                    // Originating inside the sphere
                    inside = true;
                }
                else
                {
                    tempT -= sqrtDeterminant;
                }
            }

            var intersection = new Intersection() { T = tempT, ContactObject = this, ContactMaterial = (Material)SurfaceMaterial.Clone(), Position = ray + tempT, Inside = inside };
            intersection.Normal = Vector3.Normalize(intersection.Position - Center);

            return intersection;
        }
    }
}

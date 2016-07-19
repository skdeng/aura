using Aura.VecMath;
using System;

namespace Aura.Shape
{
    class Sphere : Primitive
    {
        public Vec3 Center { get; set; }
        public double Radius { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            Vec3 oc = ray.Position - Center;
            double ocLength = oc.Length;
            double directionDotOC = ray.Direction.Dot(oc);
            double determinant = directionDotOC * directionDotOC - ocLength * ocLength + Radius * Radius;

            // No intersection
            if (determinant < 0)
            {
                return new Intersection() { Intersect = false };
            }

            double tempT = -directionDotOC;

            // Determinant larger than zero => 2 intersections
            bool inside = false;
            if (determinant > 0)
            {
                double sqrtDeterminant = Math.Sqrt(determinant);

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

            if (tempT < ray.Min || tempT > ray.Max)
            {
                return new Intersection() { Intersect = false };
            }

            var intersection = new Intersection() { Intersect = true, T = tempT, ContactObject = this, ContactMaterial = (Material)SurfaceMaterial.Clone(), Position = ray + tempT, Inside = inside };
            intersection.Normal = (intersection.Position - Center).Normalize();

            return intersection;
        }
    }
}

using Aura.Values;
using System;
using System.Numerics;

namespace Aura.Shape
{
    class AABB : Primitive
    {
        public Vector3 MinimumPoint { get; set; }

        public Vector3 MaximumPoint { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            var transformedRay = TransformRay(ray);

            var difference = transformedRay.InverseDirection;

            var tMin = (MinimumPoint - transformedRay.Position) * difference;
            var tMax = (MaximumPoint - transformedRay.Position) * difference;

            // Temporary variable for swapping
            float temp;
            if (difference.X < 0)
            {
                temp = tMin.X;
                tMin.X = tMax.X;
                tMax.X = temp;
            }
            if (difference.Y < 0)
            {
                temp = tMin.Y;
                tMin.Y = tMax.Y;
                tMax.Y = temp;
            }
            if (difference.Z < 0)
            {
                temp = tMin.Z;
                tMin.Z = tMax.Z;
                tMax.Z = temp;
            }

            var maxTMin = tMin.Max();
            var minTMax = tMax.Min();

            if (minTMax < 0 || maxTMin > minTMax)
            {
                return null;
            }

            var finalT = maxTMin > 0 ? maxTMin : minTMax;

            var intersectionPoint = transformedRay + finalT;
            Vector3 normal;
            if (Math.Abs(intersectionPoint.X - MinimumPoint.X) < Constant.Epsilon)
            {
                normal = -Vector3.UnitX;
            }
            else if (Math.Abs(intersectionPoint.X - MaximumPoint.X) < Constant.Epsilon)
            {
                normal = Vector3.UnitX;
            }
            else if (Math.Abs(intersectionPoint.Y - MinimumPoint.Y) < Constant.Epsilon)
            {
                normal = -Vector3.UnitY;
            }
            else if (Math.Abs(intersectionPoint.Y - MaximumPoint.Y) < Constant.Epsilon)
            {
                normal = Vector3.UnitY;
            }
            else if (Math.Abs(intersectionPoint.Z - MinimumPoint.Z) < Constant.Epsilon)
            {
                normal = -Vector3.UnitZ;
            }
            else // if (Math.Abs(intersectionPoint.Z - MaximumPoint.Z) < Constant.Epsilon)
            {
                normal = Vector3.UnitZ;
            }

            if (HasTransform)
            {
                normal = Vector3.TransformNormal(normal, Transform);
            }

            return new Intersection()
            {
                T = finalT,
                ContactObject = this,
                ContactMaterial = SurfaceMaterial,
                Position = intersectionPoint,
                Normal = normal,
                Inside = maxTMin < 0
            };
        }
    }
}

using Aura.VecMath;

namespace Aura.Shape
{
    class AABB : Primitive
    {
        public Vec3 MinimumPoint { get; set; }

        public Vec3 MaximumPoint { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            Vec3 difference = ray.InverseDirection;

            Vec3 tMin = (MinimumPoint - ray.Position) * difference;
            Vec3 tMax = (MaximumPoint - ray.Position) * difference;

            // Temporary variable for swapping
            double temp;
            if (difference.X < 0)
            {
                temp = tMin.X;
                tMin.X = tMax.X;
                tMax.X = tMin.X;
            }
            if (difference.Y < 0)
            {
                temp = tMin.Y;
                tMin.Y = tMax.Y;
                tMax.Y = tMin.Y;
            }
            if (difference.Z < 0)
            {
                temp = tMin.Z;
                tMin.Z = tMax.Z;
                tMax.Z = tMin.Z;
            }

            var maxTMin = tMin.Max();
            var minTMax = tMax.Min();

            if (maxTMin < 0 || minTMax < 0 || maxTMin > minTMax)
            {
                return new Intersection() { Intersect = false };
            }
            else
            {
                return new Intersection()
                {
                    Intersect = true,
                    T = maxTMin,
                    ContactObject = this,
                    ContactMaterial = SurfaceMaterial,
                    Position = ray + maxTMin
                    //Normal =
                };
            }
        }
    }
}

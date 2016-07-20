using Aura.Values;
using Aura.VecMath;

namespace Aura.Shape
{
    class Triangle : Primitive
    {
        private Vec3 _A;
        public Vec3 A
        {
            get
            {
                return _A;
            }
            set
            {
                _A = value;
                AB = B - _A;
                AC = C - _A;
            }
        }

        private Vec3 _B;
        public Vec3 B
        {
            get
            {
                return _B;
            }
            set
            {
                _B = value;
                AB = _B - A;
            }
        }

        private Vec3 _C;
        public Vec3 C
        {
            get
            {
                return _C;
            }
            set
            {
                _C = value;
                AC = _C - A;
            }
        }

        private Vec3 _AB;
        public Vec3 AB
        {
            get
            {
                return _AB;
            }
            set
            {
                _AB = value;
                Normal = _AB.Cross(AC);
            }
        }

        private Vec3 _AC;
        public Vec3 AC
        {
            get
            {
                return _AC;
            }
            set
            {
                _AC = value;
                Normal = AB.Cross(_AC);
            }
        }

        private Vec3 Normal { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            var angle = ray.Direction.Dot(Normal);
            if (angle > -Constant.PlaneHorizon && angle < Constant.PlaneHorizon)    // tangent = no intersection
            {
                return new Intersection() { Intersect = false };
            }

            var tempT = (A.Dot(Normal) - ray.Position.Dot(Normal)) / angle;
            if (tempT < ray.Min || tempT > ray.Max)
            {
                return new Intersection() { Intersect = false };
            }

            var intersectionPoint = ray + tempT;

            if (!Inside(intersectionPoint))
            {
                return new Intersection() { Intersect = false };
            }

            return new Intersection()
            {
                Intersect = true,
                T = tempT,
                Position = intersectionPoint,
                Normal = Normal,
                ContactMaterial = SurfaceMaterial,
                ContactObject = this
            };
        }

        private bool Inside (Vec3 point)
        {
            var barycentricPoint = Barycentric(point);
            return barycentricPoint.Min() > 0 || barycentricPoint.Max() < 1;
        }

        /// <summary>
        /// Get the barycentric coordinate of a point w.r.t. the triangle
        /// More info: https://en.wikipedia.org/wiki/Barycentric_coordinate_system
        /// </summary>
        /// <param name="point">Point in world space</param>
        /// <returns>Barycentric coordinate of the point in triangle space</returns>
        private Vec3 Barycentric (Vec3 point)
        {
            var pa = point - A;
            var d00 = AB.Dot(AB);
            var d01 = AB.Dot(AC);
            var d11 = AC.Dot(AC);
            var d20 = pa.Dot(AB);
            var d21 = pa.Dot(AC);

            var denominator = d00 * d11 - d01 * d01;
            var v = new Vec3()
            {
                Y = (d11 * d20 - d01 * d21) / denominator,
                Z = (d00 * d21 - d01 * d20) / denominator
            };
            v.X = 1 - v.Y - v.Z;
            return v;
        }
    }
}

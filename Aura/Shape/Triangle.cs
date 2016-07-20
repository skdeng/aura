using Aura.Values;
using System.Numerics;

namespace Aura.Shape
{
    class Triangle : Primitive
    {
        private Vector3 _A;
        public Vector3 A
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

        private Vector3 _B;
        public Vector3 B
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

        private Vector3 _C;
        public Vector3 C
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

        private Vector3 _AB;
        public Vector3 AB
        {
            get
            {
                return _AB;
            }
            set
            {
                _AB = value;
                Normal = Vector3.Cross(_AB, AC);
            }
        }

        private Vector3 _AC;
        public Vector3 AC
        {
            get
            {
                return _AC;
            }
            set
            {
                _AC = value;
                Normal = Vector3.Cross(AB, _AC);
            }
        }

        private Vector3 Normal { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            var angle = Vector3.Dot(ray.Direction, Normal);
            if (angle > -Constant.PlaneHorizon && angle < Constant.PlaneHorizon)    // tangent = no intersection
            {
                return null;
            }

            var tempT = (Vector3.Dot(A, Normal) - Vector3.Dot(ray.Position, Normal)) / angle;

            var intersectionPoint = ray + tempT;

            if (!Inside(intersectionPoint))
            {
                return null;
            }

            return new Intersection()
            {
                T = tempT,
                Position = intersectionPoint,
                Normal = Normal,
                ContactMaterial = SurfaceMaterial,
                ContactObject = this
            };
        }

        private bool Inside (Vector3 point)
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
        private Vector3 Barycentric (Vector3 point)
        {
            var pa = point - A;
            var d00 = Vector3.Dot(AB, AB);
            var d01 = Vector3.Dot(AB, AC);
            var d11 = Vector3.Dot(AC, AC);
            var d20 = Vector3.Dot(pa, AB);
            var d21 = Vector3.Dot(pa, AC);

            var denominator = d00 * d11 - d01 * d01;
            var v = new Vector3()
            {
                Y = (d11 * d20 - d01 * d21) / denominator,
                Z = (d00 * d21 - d01 * d20) / denominator
            };
            v.X = 1 - v.Y - v.Z;
            return v;
        }
    }
}

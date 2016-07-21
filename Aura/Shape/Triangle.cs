using Aura.Values;
using System.Numerics;

namespace Aura.Shape
{
    internal class Triangle : Primitive
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
                d00 = Vector3.Dot(_AB, _AB);
                d01 = Vector3.Dot(_AB, AC);
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
                d01 = Vector3.Dot(AB, _AC);
                d11 = Vector3.Dot(_AC, _AC);
            }
        }

        private float d00;
        private float d01;
        private float d11;

        public Vector3 Normal { get; set; }

        public override Intersection Intersect(Ray ray)
        {
            var cosAngle = Vector3.Dot(ray.Direction, Normal);
            if (cosAngle > -Constant.PlaneHorizon && cosAngle < Constant.PlaneHorizon)    // tangent = no intersection
            {
                return null;
            }

            var tempT = (Vector3.Dot(A, Normal) - Vector3.Dot(ray.Position, Normal)) / cosAngle;

            if (tempT < 0)
            {
                return null;
            }

            var intersectionPoint = ray + tempT;

            if (!Inside(intersectionPoint))
            {
                return null;
            }

            return new Intersection()
            {
                T = tempT,
                Position = intersectionPoint,
                Normal = cosAngle < 0 ? -Normal : Normal,
                ContactMaterial = SurfaceMaterial,
                ContactObject = this,
                Inside = cosAngle < 0
            };
        }

        private bool Inside (Vector3 point)
        {
            var barycentricPoint = Barycentric(point);
            return barycentricPoint.Min() >= 0 && barycentricPoint.Max() <= 1;
        }

        /// <summary>
        /// Get the barycentric coordinate of a point w.r.t. the triangle
        /// More info: https://en.wikipedia.org/wiki/Barycentric_coordinate_system
        /// Algorithm from: http://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
        /// </summary>
        /// <param name="point">Point in world space</param>
        /// <returns>Barycentric coordinate of the point in triangle space</returns>
        private Vector3 Barycentric (Vector3 point)
        {
            var pa = point - A;
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

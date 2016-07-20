using Aura.Values;
using System.Numerics;

namespace Aura
{
    class Ray
    {
        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public Vector3 InverseDirection { get; }
        public float Min { get; }
        public float Max { get; }

        public Ray()
        {
            Position = new Vector3();
            Direction = new Vector3();
        }

        public Ray(Vector3 position, Vector3 direction) : this(position, direction, Constant.RayMinimum, Constant.RayMaximum)
        {
        }

        public Ray(Vector3 position, Vector3 direction, float min, float max)
        {
            Position = position;
            Direction = direction;
            Min = min;
            Max = max;

            InverseDirection = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
        }

        public static Vector3 operator +(Ray r, float t)
        {
            return r.Position + t * r.Direction;
        }
    }
}

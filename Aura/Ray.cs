using Aura.Values;
using System.Numerics;

namespace Aura
{
    class Ray
    {
        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public Vector3 InverseDirection { get; }

        public Ray()
        {
            Position = new Vector3();
            Direction = new Vector3();
        }
        
        public Ray(Vector3 position, Vector3 direction)
        {
            Position = position + direction * Constant.RayMinimum;
            Direction = direction;

            InverseDirection = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
        }

        public static Vector3 operator +(Ray r, float t)
        {
            return r.Position + t * r.Direction;
        }
    }
}

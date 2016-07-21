using Aura.Values;
using System.Numerics;

namespace Aura
{
    internal class Ray
    {
        public Vector3 Position { get; }
        public Vector4 PositionHomogenous { get; }

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

            PositionHomogenous = new Vector4(position, 1.0f);
            InverseDirection = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
        }

        public Ray(Vector4 position, Vector3 direction)
        {
            PositionHomogenous = position;
            Direction = direction;

            Position = new Vector3(PositionHomogenous.X, PositionHomogenous.Y, PositionHomogenous.Z);
            InverseDirection = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
        }

        public static Vector3 operator +(Ray r, float t)
        {
            return r.Position + t * r.Direction;
        }
    }
}

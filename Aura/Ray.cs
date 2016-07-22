using Aura.Values;
using System.Numerics;

namespace Aura
{
    internal class Ray
    {
        public Vector3 Position { get; }
        

        public Vector3 Direction { get; }
        public Vector3 InverseDirection { get; }
        public Vector4 DirectionHomogenous { get; }

        public Ray()
        {
            Position = new Vector3();
            Direction = new Vector3();
        }
        
        public Ray(Vector3 position, Vector3 direction)
        {
            Position = position + direction * Constant.RayMinimum;
            Direction = direction;

            DirectionHomogenous = new Vector4(direction, 0.0f);
            InverseDirection = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
        }

        public Ray(Vector3 position, Vector4 direction)
        {
            Position = position;
            DirectionHomogenous = direction;

            Direction = new Vector3(DirectionHomogenous.X, DirectionHomogenous.Y, DirectionHomogenous.Z);
            InverseDirection = new Vector3(1 / direction.X, 1 / direction.Y, 1 / direction.Z);
        }

        public static Vector3 operator +(Ray r, float t)
        {
            return r.Position + t * r.Direction;
        }
    }
}

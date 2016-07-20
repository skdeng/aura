using Aura.Values;
using Aura.VecMath;

namespace Aura
{
    class Ray
    {
        public Vec3 Position { get; }
        public Vec3 Direction { get; }
        public Vec3 InverseDirection { get; }
        public double Min { get; }
        public double Max { get; }

        public Ray()
        {
            Position = new Vec3();
            Direction = new Vec3();
        }

        public Ray(Vec3 position, Vec3 direction) : this(position, direction, Constant.RayMinimum, Constant.RayMaximum)
        {
        }

        public Ray(Vec3 position, Vec3 direction, double min, double max)
        {
            Position = position;
            Direction = direction;
            Min = min;
            Max = max;

            InverseDirection = 1 / direction;
        }

        public static Vec3 operator +(Ray r, double t)
        {
            return r.Position + t * r.Direction;
        }
    }
}

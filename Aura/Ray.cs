using Aura.Values;
using Aura.VecMath;

namespace Aura
{
    class Ray
    {
        public Vec3 Position { get; set; }
        public Vec3 Direction { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }

        public Ray()
        {
            Position = new Vec3();
            Direction = new Vec3();
        }

        public Ray(Vec3 position, Vec3 direction)
        {
            Position = position;
            Direction = direction;
            Min = Constant.RayMinimum;
            Max = Constant.RayMaximum;
        }

        public Ray(Vec3 position, Vec3 direction, double min, double max)
        {
            Position = position;
            Direction = direction;
            Min = min;
            Max = max;
        }

        public static Vec3 operator +(Ray r, double t)
        {
            return r.Position + t * r.Direction;
        }
    }
}

using Aura.Shape;
using Aura.VecMath;

namespace Aura
{
    class Intersection
    {
        public bool Intersect { get; set; }
        public double T { get; set; }
        public Vec3 Position { get; set; }
        public Vec3 Normal { get; set; }
        public Material ContactMaterial{ get; set; }
        public Primitive ContactObject { get; set; }
        public int RecursionDepth { get; set; }
        public bool Inside { get; set; }
    }
}

using Aura.Shape;
using System.Numerics;

namespace Aura
{
    class Intersection
    {
        public bool Intersect { get; set; }
        public double T { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Material ContactMaterial{ get; set; }
        public Primitive ContactObject { get; set; }
        public bool Inside { get; set; }
    }
}

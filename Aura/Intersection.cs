using Aura.Shape;
using System.Numerics;

namespace Aura
{
    class Intersection
    {
        public float T { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Material ContactMaterial{ get; set; }
        public Primitive ContactObject { get; set; }
        public bool Inside { get; set; }
    }
}

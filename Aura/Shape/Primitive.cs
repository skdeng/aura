using Aura.VecMath;

namespace Aura.Shape
{
    abstract class Primitive : IIntersectable
    {
        public string Name { get; set; }
        public Material SurfaceMaterial { get; set; }
        public Mat4 Transform { get; set; }

        abstract public Intersection Intersect(Ray ray);
    }
}

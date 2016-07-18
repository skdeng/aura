namespace Aura.Shape
{
    abstract class Primitive
    {
        public string Name { get; set; }
        public Material SurfaceMaterial { get; set; }

        abstract public Intersection Intersect(Ray ray);
    }
}

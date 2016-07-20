using System.Numerics;

namespace Aura.Shape
{
    abstract class Primitive : IIntersectable
    {
        public string Name { get; set; }
        public Material SurfaceMaterial { get; set; }

        public bool HasTransform { get; set; }

        private Matrix4x4 _Transform;
        public Matrix4x4 Transform
        {
            get
            {
                return _Transform;
            }
            set
            {
                _Transform = value;
                HasTransform = _Transform.IsIdentity;
            }
        }

        abstract public Intersection Intersect(Ray ray);

        protected Ray TransformRay(Ray ray)
        {
            if (HasTransform)
            {
                return new Ray(Vector4.Transform(ray.PositionHomogenous, Transform), Vector3.Transform(ray.Direction, Transform));
            }
            else
            {
                return ray;
            }
        }
    }
}

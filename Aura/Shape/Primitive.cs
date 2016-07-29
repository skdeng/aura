using System.Numerics;

namespace Aura.Shape
{
    abstract class Primitive : IIntersectable
    {
        public string Name { get; set; }
        public Material SurfaceMaterial { get; set; }

        public bool HasTransform { get; set; }

        private Matrix4x4 _Transform;
        private Matrix4x4 _TransformInverse;
        public virtual Matrix4x4 Transform
        {
            get
            {
                return _Transform;
            }
            set
            {
                _Transform = value;
                Matrix4x4.Invert(_Transform, out _TransformInverse);
                HasTransform = !_Transform.IsIdentity;
            }
        }

        abstract public Intersection Intersect(Ray ray);

        protected Ray TransformRay(Ray ray)
        {
            if (HasTransform)
            {
                return new Ray(Vector3.Transform(ray.Position, _TransformInverse), Vector4.Transform(ray.DirectionHomogenous, _TransformInverse));
            }
            else
            {
                return ray;
            }
        }
    }
}

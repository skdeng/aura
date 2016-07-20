using System.Collections.Generic;

namespace Aura.Shape
{
    class Mesh : Primitive
    {
        public readonly List<Triangle> Triangles;

        public AABB BoundingBox { get; set; }

        public Mesh()
        {
            Triangles = new List<Triangle>();
        }

        public override Intersection Intersect(Ray ray)
        {
            if (!BoundingBox.Intersect(ray).Intersect)
            {
                return new Intersection() { Intersect = false };
            }

            Intersection finalIntersection = new Intersection() { Intersect = false, T = double.PositiveInfinity };

            foreach (var triangle in Triangles)
            {
                var tempIntersection = triangle.Intersect(ray);
                if (tempIntersection.Intersect)
                {
                    if (tempIntersection.T < finalIntersection.T)
                    {
                        finalIntersection = tempIntersection;
                    }
                }
            }

            return finalIntersection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Shape
{
    abstract class Primitive
    {
        abstract public Intersection Intersect(Ray ray);
        public Material SurfaceMaterial { get; set; }
    }
}

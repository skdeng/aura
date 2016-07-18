using Aura.VecMath;
using System;

namespace Aura
{
    class Material : ICloneable
    {
        public enum MaterialType
        {
            Diffuse,
            Reflective,
            Refractive,
            Emissive
        }

        public Vec3 Emission { get; set; }
        public Vec3 Diffuse { get; set; }
        public Vec3 Transparency { get; set; }
        public double RefractionIndex { get; set; }
        public MaterialType Type { get; set; }

        public object Clone()
        {
            return new Material()
            {
                Emission = (Vec3)Emission.Clone(),
                Diffuse = (Vec3)Diffuse.Clone(),
                Transparency = (Vec3)Transparency.Clone(),
                RefractionIndex = RefractionIndex,
                Type = Type
            };
        }
    }
}

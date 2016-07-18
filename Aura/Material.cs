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
            Refractive
        }

        public Vec3 Emission { get; set; }
        public Vec3 Diffuse { get; set; }
        public Vec3 Specular { get; set; }
        public Vec3 Reflectance { get; set; }
        public double SpecularHardness { get; set; }
        public Vec3 Transparency { get; set; }
        public double RefractionIndex { get; set; }
        public MaterialType Type { get; set; }

        public object Clone()
        {
            return new Material()
            {
                Emission = (Vec3)Emission.Clone(),
                Diffuse = (Vec3)Diffuse.Clone(),
                Specular = (Vec3)Specular.Clone(),
                Reflectance = (Vec3)Reflectance.Clone(),
                SpecularHardness = SpecularHardness,
                Transparency = (Vec3)Transparency.Clone(),
                RefractionIndex = RefractionIndex
            };
        }
    }
}

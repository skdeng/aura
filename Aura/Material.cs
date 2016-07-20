using System;
using System.Numerics;

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

        public Vector3 Emission { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Transparency { get; set; }
        public float RefractionIndex { get; set; }
        public MaterialType Type { get; set; }

        public object Clone()
        {
            return new Material()
            {
                Emission = Emission.Copy(),
                Diffuse = Diffuse.Copy(),
                Transparency = Transparency.Copy(),
                RefractionIndex = RefractionIndex,
                Type = Type
            };
        }
    }
}

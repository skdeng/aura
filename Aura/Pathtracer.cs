using Aura.Shape;
using Aura.Values;
using Aura.VecMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura
{
    class Pathtracer
    {
        private readonly Scene MainScene;

        private readonly Random RNG;

        public Pathtracer(Scene sceneDescription)
        {
            MainScene = sceneDescription;
            RNG = new Random();
        }

        public Vec3 Trace(Ray ray, int recursionDepth = 0)
        {
            var intersection = MainScene.Intersect(ray);

            if (!intersection.Intersect)
            {
                return MainScene.BackgroundColor;
            }

            var contactMaterial = intersection.ContactMaterial;

            if (recursionDepth > 50)
            {
                return contactMaterial.Emission;
            }

            var maxReflectance = contactMaterial.Diffuse.Max();

            if (++recursionDepth > MainScene.RecursiveDepthLimit)
            {
                if (RNG.NextDouble() < maxReflectance)
                {
                    contactMaterial.Diffuse /= maxReflectance;
                }
                else
                {
                    return contactMaterial.Emission;
                }
            }

            Ray reflectedRay = new Ray(intersection.Position, Vec3.RandomHemisphereVector(intersection.Normal, RNG));

            return contactMaterial.Emission + contactMaterial.Diffuse * Trace(reflectedRay, recursionDepth);
            
        }
    }
}

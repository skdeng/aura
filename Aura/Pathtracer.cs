using Aura.VecMath;
using System;

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

            switch (contactMaterial.Type)
            {
                case Material.MaterialType.Diffuse:
                    return DiffuseRadiance(ray, intersection, recursionDepth);

                case Material.MaterialType.Reflective:
                    return ReflectiveRadiance(ray, intersection, recursionDepth);

                case Material.MaterialType.Refractive:
                    return RefractionRadiance(ray, intersection, recursionDepth);

                case Material.MaterialType.Emissive:
                    return contactMaterial.Emission;

                default:
                    throw new InvalidOperationException("Unknown material type: " + contactMaterial.Type.ToString());
            }
        }

        private Vec3 DiffuseRadiance(Ray ray, Intersection intersection, int recursionDepth)
        {
            var contactMaterial = intersection.ContactMaterial;
            var reflectedRay = new Ray(intersection.Position, Vec3.RandomHemisphereVector_Uniform(intersection.Normal, RNG));
            var cosTheta = reflectedRay.Direction.Dot(intersection.Normal);
            var brdf = 2 * contactMaterial.Diffuse * cosTheta;
            return contactMaterial.Emission + brdf * Trace(reflectedRay, recursionDepth);
        }

        private Vec3 ReflectiveRadiance(Ray ray, Intersection intersection, int recursionDepth)
        {
            var contactMaterial = intersection.ContactMaterial;
            var reflectedRay = new Ray(intersection.Position, ray.Direction.Reflect(intersection.Normal));
            return contactMaterial.Emission + contactMaterial.Diffuse * Trace(reflectedRay, recursionDepth);
        }

        private Vec3 RefractionRadiance(Ray ray, Intersection intersection, int recursionDepth)
        {
            var contactMaterial = intersection.ContactMaterial;

            var reflectedRay = new Ray(intersection.Position, ray.Direction.Reflect(intersection.Normal));

            var inside = contactMaterial.RefractionIndex < 1;

            var incidentAngle = ray.Direction.Dot(intersection.Normal);

            // Total internal reflection
            var cos2T = 1 - contactMaterial.RefractionIndex * contactMaterial.RefractionIndex * (1 - incidentAngle * incidentAngle);
            if (cos2T < 0)
            {
                return contactMaterial.Emission + contactMaterial.Diffuse * Trace(reflectedRay, recursionDepth);
            }

            var refractedRay = new Ray(intersection.Position, (ray.Direction * contactMaterial.RefractionIndex - intersection.Normal * (inside ? -1 : 1) * (incidentAngle * contactMaterial.RefractionIndex + Math.Sqrt(cos2T))).Normalize());

            var A = contactMaterial.RefractionIndex - 1;
            var B = contactMaterial.RefractionIndex + 1;
            var R = A * A / (B * B);

            var C = 1 - (inside ? refractedRay.Direction.Dot(intersection.Normal) : -incidentAngle);
            var re = R + (1 - R) * Math.Pow(C, 5);
            var tr = 1 - re;
            var pp = 0.25 + 0.5 * re;
            var rp = re / pp;
            var tp = tr / (1 - pp);

            return contactMaterial.Emission + contactMaterial.Diffuse * (
                recursionDepth > 2 ?
                    (RNG.NextDouble() < pp ? Trace(reflectedRay, recursionDepth) * rp : Trace(refractedRay, recursionDepth)) :
                    Trace(reflectedRay, recursionDepth) * re + Trace(refractedRay, recursionDepth) * tr                        
                );
        }
    }
}

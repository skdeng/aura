using System;
using System.Numerics;

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

        public Vector3 Trace(Ray ray, int recursionDepth = 0)
        {
            var intersection = MainScene.Intersect(ray);

            if (!intersection.Intersect)
            {
                return MainScene.BackgroundColor;
            }

            var contactMaterial = intersection.ContactMaterial;

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

        private Vector3 DiffuseRadiance(Ray ray, Intersection intersection, int recursionDepth)
        {
            var contactMaterial = intersection.ContactMaterial;
            var reflectedRay = new Ray(intersection.Position, Rand.RandomHemisphereVector_Uniform(intersection.Normal));
            var cosTheta = Vector3.Dot(reflectedRay.Direction, intersection.Normal);
            var brdf = 2 * contactMaterial.Diffuse * cosTheta;
            return contactMaterial.Emission + brdf * Trace(reflectedRay, recursionDepth);
        }

        private Vector3 ReflectiveRadiance(Ray ray, Intersection intersection, int recursionDepth)
        {
            var contactMaterial = intersection.ContactMaterial;
            var reflectedRay = new Ray(intersection.Position, Vector3.Reflect(ray.Direction, intersection.Normal));
            return contactMaterial.Emission + contactMaterial.Diffuse * Trace(reflectedRay, recursionDepth);
        }

        private Vector3 RefractionRadiance(Ray ray, Intersection intersection, int recursionDepth)
        {
            var contactMaterial = intersection.ContactMaterial;

            var reflectedRay = new Ray(intersection.Position, Vector3.Reflect(ray.Direction, intersection.Normal));
            var cosIncidentAngle = Vector3.Dot(ray.Direction, intersection.Inside ? -intersection.Normal : intersection.Normal);

            var refractionIndexAir = 1;
            var refractionIndexObj = contactMaterial.RefractionIndex;
            var indexRatio = intersection.Inside ? refractionIndexObj / refractionIndexAir : refractionIndexAir / refractionIndexObj;

            // Total internal reflection
            var cos2T = 1 - indexRatio * indexRatio * (1 - cosIncidentAngle * cosIncidentAngle);
            if (cos2T < 0)
            {
                return contactMaterial.Emission + contactMaterial.Diffuse * Trace(reflectedRay, recursionDepth);
            }

            var refractionDirection = Vector3.Normalize(ray.Direction * indexRatio - intersection.Normal * ((intersection.Inside) ? -1 : 1) * (cosIncidentAngle * indexRatio + (float)Math.Sqrt(cos2T)));
            var refractedRay = new Ray(intersection.Position, refractionDirection);

            var A = refractionIndexObj - refractionIndexAir;
            var B = refractionIndexObj + refractionIndexAir;
            var R = A * A / (B * B);

            var C = 1 - (intersection.Inside ? Vector3.Dot(refractionDirection, intersection.Normal) : -cosIncidentAngle);
            var re = R + (1 - R) * (float)Math.Pow(C, 5);
            var tr = 1 - re;
            var pp = 0.25f + 0.5f * re;
            var rp = re / pp;
            var tp = tr / (1 - pp);

            return contactMaterial.Emission + contactMaterial.Diffuse * (
                recursionDepth > 4 ?
                    (RNG.NextDouble() < pp ? Trace(reflectedRay, recursionDepth) * rp : Trace(refractedRay, recursionDepth)) :
                    Trace(reflectedRay, recursionDepth) * re + Trace(refractedRay, recursionDepth) * tr                        
                );
        }
    }
}

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

            //var r1 = 2 * Math.PI * RNG.NextDouble();
            //var r2 = RNG.NextDouble();
            //var r2SquareRoot = Math.Sqrt(r2);
            //var w = intersection.Normal;
            //var u = (Math.Abs(w.X) > .1 ? new Vec3(0,1,0): new Vec3(1,0,0).Cross(w)).Normalize();
            //var v = w.Cross(u);
            //var d = (u * Math.Cos(r1) * r2SquareRoot + v * Math.Sin(r1) * r2SquareRoot + w * Math.Sqrt(1 - r2)).Normalize();
            //return contactMaterial.Emission + contactMaterial.Diffuse * Trace(new Ray(intersection.Position, d), recursionDepth);
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
            var cosIncidentAngle = ray.Direction.Dot(intersection.Inside ? -intersection.Normal : intersection.Normal);

            var refractionIndexAir = 1;
            var refractionIndexObj = contactMaterial.RefractionIndex;
            var indexRatio = intersection.Inside ? refractionIndexObj / refractionIndexAir : refractionIndexAir / refractionIndexObj;

            // Total internal reflection
            var cos2T = 1 - indexRatio * indexRatio * (1 - cosIncidentAngle * cosIncidentAngle);
            if (cos2T < 0)
            {
                return contactMaterial.Emission + contactMaterial.Diffuse * Trace(reflectedRay, recursionDepth);
            }

            var refractionDirection = (ray.Direction * indexRatio - intersection.Normal * ((intersection.Inside) ? -1 : 1) * (cosIncidentAngle * indexRatio + Math.Sqrt(cos2T))).Normalize();
            //var refractionDirection = ray.Direction.Refract(intersection.Inside ? -intersection.Normal : intersection.Normal, contactMaterial.RefractionIndex);
            var refractedRay = new Ray(intersection.Position, refractionDirection);

            var A = refractionIndexObj - refractionIndexAir;
            var B = refractionIndexObj + refractionIndexAir;
            var R = A * A / (B * B);

            var C = 1 - (intersection.Inside ? refractionDirection.Dot(intersection.Normal) : -cosIncidentAngle);
            var re = R + (1 - R) * Math.Pow(C, 5);
            var tr = 1 - re;
            var pp = 0.25 + 0.5 * re;
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

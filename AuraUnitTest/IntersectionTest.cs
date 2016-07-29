using Aura;
using Aura.Shape;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;

namespace AuraUnitTest
{
    [TestClass]
    public class IntersectionTest
    {
        [TestMethod]
        public void TriangleIntersection()
        {
            var tri = new Triangle()
            {
                A = new Vector3(1, 0, -2),
                B = new Vector3(0, 1, -2),
                C = new Vector3(-1, 0, -2)
            };

            var rayPos = new Vector3(0, 0, 0);
            var ray = new Ray(rayPos, Vector3.Normalize(tri.A - rayPos));
            Assert.IsNotNull(tri.Intersect(ray));
            ray = new Ray(rayPos, Vector3.Normalize(tri.B - rayPos));
            Assert.IsNotNull(tri.Intersect(ray));
            ray = new Ray(rayPos, Vector3.Normalize(tri.C - rayPos));
            Assert.IsNotNull(tri.Intersect(ray));
        }

        [TestMethod]
        public void MeshIntersection()
        {
            var model = Model.LoadModel("..\\..\\..\\Scene\\models\\test.obj", Matrix4x4.Identity);

            var transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationX((float)Math.PI / 2), Matrix4x4.CreateTranslation(0, 1, 0));
            model.Transform = transform;

            var rayPos = new Vector3(0, 0, 0);
            var rayTarget = new Vector3(-1, -1, 1);
            var ray = new Ray(rayPos, Vector3.Normalize(rayTarget - rayPos));
            Assert.IsNull(model.Intersect(ray), "Mesh shouldn't intersect after transform");

            rayTarget.Y = 1;
            ray = new Ray(rayPos, Vector3.Normalize(rayTarget - rayPos));
            var intersection = model.Intersect(ray);
            Assert.IsNotNull(intersection, "Inside intersection failed");
            Assert.IsTrue(intersection.Inside, "Inside intersection inside test failed");
            Assert.IsTrue(intersection.Normal.Z.FuzzyEqual(-1.0f));

            rayPos = new Vector3(0, 1, 3);
            var rayDir = new Vector3(0, 0, -1);
            ray = new Ray(rayPos, rayDir);
            intersection = model.Intersect(ray);
            Assert.IsNotNull(intersection, "Direct interseciton failed");
            Assert.IsFalse(intersection.Inside, "Direct intersection shouldn't be inside");
            Assert.IsTrue(1.0f.FuzzyEqual(intersection.T), "Direct intersection point is wrong");
            Assert.IsTrue(intersection.Normal.Z.FuzzyEqual(1.0f));
        }

        [TestMethod]
        public void SphereIntersection()
        {
            var sphere = new Sphere()
            {
                Center = new Vector3(0, 0, 0),
                Radius = 1,
                SurfaceMaterial = new Material()

            };

            var rayPos = new Vector3(0, 0, 3);
            var rayDir = new Vector3(0, 0, -1);
            var ray = new Ray(rayPos, rayDir);
            var intersection = sphere.Intersect(ray);
            Assert.IsNotNull(intersection, "Direct intersection returned null");
            Assert.IsFalse(intersection.Inside, "Direct intersection should not be inside");

            rayPos = new Vector3(0, 1, 3);
            rayDir = new Vector3(0, 0, -1);
            ray = new Ray(rayPos, rayDir);
            intersection = sphere.Intersect(ray);
            Assert.IsNull(intersection, "Tangential intersection returned null");

            var rng = new Random();
            for (int i = 0; i < 5; i++)
            {
                rayPos = new Vector3(0, 0, 0);
                rayDir = new Vector3((float)rng.NextDouble() * 2 - 1, (float)rng.NextDouble() * 2 - 1, (float)rng.NextDouble() * 2 - 1);
                ray = new Ray(rayPos, rayDir);
                intersection = sphere.Intersect(ray);
                Assert.IsNotNull(intersection, "Inside intersection returned null");
                Assert.IsTrue(intersection.Inside, "Inside intersection should be inside");
                Assert.IsTrue(1.0f.FuzzyEqual(intersection.T), "Inside intersection point is wrong");
            }
        }

        [TestMethod]
        public void AABBIntersection()
        {
            var box = new AABB()
            {
                MinimumPoint = new Vector3(-1, -1, -1),
                MaximumPoint = new Vector3(1, 1, 1)
            };

            var rayPos = new Vector3(0, 0, 3);
            var rayDir = new Vector3(0, 0, -1);
            var ray = new Ray(rayPos, rayDir);
            var intersection = box.Intersect(ray);
            Assert.IsNotNull(intersection);
        }
    }
}

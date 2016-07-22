using Aura;
using Aura.Shape;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var model = Model.LoadModel("..\\..\\..\\Scene\\models\\test.obj");

            model.Transform = Matrix4x4.CreateTranslation(0, 1, 0);

            var rayPos = new Vector3(0, 0, 0);
            var rayTarget = new Vector3(-1, -1, 1);
            var ray = new Ray(rayPos, Vector3.Normalize(rayTarget - rayPos));

            Assert.IsNull(model.Intersect(ray), "Mesh shouldn't intersect after transform");

            rayTarget.Y = 0;
            ray = new Ray(rayPos, Vector3.Normalize(rayTarget - rayPos));

            Assert.IsNotNull(model.Intersect(ray), "Mesh intersection failed");
        }
    }
}

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
            var mesh = Mesh.LoadOBJ("E:\\aura\\Scene\\test.obj");

            var rayPos = new Vector3(0, 0, 0);
            var ray = new Ray(rayPos, Vector3.Normalize(mesh.Triangles[0].A - rayPos));

            Assert.IsNotNull(mesh.Triangles[0].Intersect(ray), "Triangle intersection failed");

            Assert.IsNotNull(mesh.Intersect(ray), "Mesh intersection failed");
        }
    }
}

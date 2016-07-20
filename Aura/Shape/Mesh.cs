using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Aura.Shape
{
    class Mesh : Primitive
    {
        public readonly List<Vector3> Vertices;

        public readonly List<Vector3> Normals;

        public readonly List<Triangle> Triangles;

        public AABB BoundingBox { get; set; }

        public Mesh()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            Triangles = new List<Triangle>();
        }

        public override Intersection Intersect(Ray ray)
        {
            if (BoundingBox.Intersect(ray) != null)
            {
                return null;
            }

            Intersection finalIntersection = null;

            foreach (var triangle in Triangles)
            {
                var tempIntersection = triangle.Intersect(ray);
                if (tempIntersection != null)
                {
                    if (finalIntersection == null || tempIntersection.T < finalIntersection.T)
                    {
                        finalIntersection = tempIntersection;
                    }
                }
            }

            return finalIntersection;
        }

        public void LoadOBJ(string objFile)
        {
            var content = File.ReadAllLines(objFile);

            var faceRegex = new Regex("f (\\d+)/(\\d+)/(\\d+) (\\d+)/(\\d+)/(\\d+) (\\d+)/(\\d+)/(\\d+)");
            foreach(var line in content)
            {
                if (line.StartsWith("v "))
                {
                    var tokens = line.Split(' ');
                    Vertices.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                }
                else if (line.StartsWith("vn "))
                {
                    var tokens = line.Split(' ');
                    Normals.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                }
                else if (line.StartsWith("f "))
                {
                    var match = faceRegex.Match(line);
                    int[] indices = new int[9];
                    for (int i = 1; i < 10; i++)
                    {
                        indices[i - 1] = int.Parse(match.Groups[i].Value);
                    }
                    var averageNormal = Vector3.Normalize(Normals[indices[2] - 1] + Normals[indices[5] - 1] + Normals[indices[8] - 1]);
                    Triangles.Add(new Triangle()
                    {
                        A = Vertices[indices[0] - 1],
                        B = Vertices[indices[3] - 1],
                        C = Vertices[indices[6] - 1],
                        Normal = averageNormal,
                        SurfaceMaterial = this.SurfaceMaterial
                    });
                }
            }
        }
    }
}

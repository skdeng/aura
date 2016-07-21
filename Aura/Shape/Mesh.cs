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
            var transformedRay = TransformRay(ray);

            if (BoundingBox.Intersect(transformedRay) == null)
            {
                return null;
            }

            Intersection finalIntersection = null;

            foreach (var triangle in Triangles)
            {
                var tempIntersection = triangle.Intersect(transformedRay);
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

        public static Mesh LoadOBJ(string objFile, Material meshMaterial)
        {
            var content = File.ReadAllLines(objFile);

            var mesh = new Mesh();
            mesh.SurfaceMaterial = meshMaterial;

            Vector3 minPoint = new Vector3(float.PositiveInfinity);
            Vector3 maxPoint = new Vector3(float.NegativeInfinity);

            var faceRegex = new Regex("f (\\d+)/(\\d+)/(\\d+) (\\d+)/(\\d+)/(\\d+) (\\d+)/(\\d+)/(\\d+)");
            foreach (var line in content)
            {
                if (line.StartsWith("v "))
                {
                    var tokens = line.Split(' ');
                    var vertex = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                    mesh.Vertices.Add(vertex);

                    if (vertex.X < minPoint.X)
                    {
                        minPoint.X = vertex.X;
                    }
                    else if (vertex.X > maxPoint.X)
                    {
                        maxPoint.X = vertex.X;
                    }
                    if (vertex.Y < minPoint.Y)
                    {
                        minPoint.Y = vertex.Y;
                    }
                    else if (vertex.Y > maxPoint.Y)
                    {
                        maxPoint.Y = vertex.Y;
                    }
                    if (vertex.Z < minPoint.Z)
                    {
                        minPoint.Z = vertex.Z;
                    }
                    else if (vertex.Z > maxPoint.Z)
                    {
                        maxPoint.Z = vertex.Z;
                    }
                }
                else if (line.StartsWith("vn "))
                {
                    var tokens = line.Split(' ');
                    mesh.Normals.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                }
                else if (line.StartsWith("f "))
                {
                    var match = faceRegex.Match(line);
                    int[] indices = new int[9];
                    for (int i = 1; i < 10; i++)
                    {
                        indices[i - 1] = int.Parse(match.Groups[i].Value);
                    }
                    var averageNormal = Vector3.Normalize(mesh.Normals[indices[2] - 1] + mesh.Normals[indices[5] - 1] + mesh.Normals[indices[8] - 1]);
                    mesh.Triangles.Add(new Triangle()
                    {
                        A = mesh.Vertices[indices[0] - 1],
                        B = mesh.Vertices[indices[3] - 1],
                        C = mesh.Vertices[indices[6] - 1],
                        Normal = averageNormal,
                        SurfaceMaterial = mesh.SurfaceMaterial
                    });
                }
            }

            mesh.BoundingBox = new AABB() { MinimumPoint = minPoint, MaximumPoint = maxPoint };

            return mesh;
        }
    }
}

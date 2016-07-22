using Aura.Values;
using System.Collections.Generic;
using System.Numerics;

namespace Aura.Shape
{
    class Model : Primitive
    {
        public readonly List<Vector3> Vertices;

        public readonly List<Vector3> Normals;

        public readonly List<Triangle> Triangles;

        public AABB BoundingBox { get; set; }

        public Model()
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

        public static Model LoadModel(string filename, Material inputMaterial = null)
        {
            var importer = new Assimp.AssimpContext();

            Assimp.Scene scene = importer.ImportFile(filename, Assimp.PostProcessPreset.TargetRealTimeMaximumQuality);

            var model = new Model();

            Vector3 minPoint = new Vector3(float.PositiveInfinity);
            Vector3 maxPoint = new Vector3(float.NegativeInfinity);

            foreach (var mesh in scene.Meshes)
            {
                Material meshMaterial;
                if (inputMaterial == null)
                {
                    var meshMat = scene.Materials[mesh.MaterialIndex];
                    var diffuseColor = meshMat.ColorDiffuse;
                    var emissionColor = meshMat.ColorEmissive;
                    var reflectance = meshMat.ColorReflective;
                    var transparency = meshMat.ColorTransparent;

                    meshMaterial = new Material()
                    {
                        Diffuse = new Vector3(diffuseColor.R, diffuseColor.G, diffuseColor.B),
                        Emission = new Vector3(emissionColor.R, emissionColor.G, emissionColor.B),
                        Transparency = new Vector3(transparency.R, transparency.G, transparency.B)
                    };
                    if (!transparency.IsBlack())
                    {
                        meshMaterial.Type = Material.MaterialType.Refractive;
                        meshMaterial.RefractionIndex = 1.5f;
                    }
                    else if (!reflectance.IsBlack())
                    {
                        meshMaterial.Type = Material.MaterialType.Reflective;
                    }
                    else if (!emissionColor.IsBlack())
                    {
                        meshMaterial.Type = Material.MaterialType.Emissive;
                    }
                    else
                    {
                        meshMaterial.Type = Material.MaterialType.Diffuse;
                        if (diffuseColor.IsBlack())
                        {
                            meshMaterial.Diffuse = Color.White;
                        }
                    }
                }
                else
                {
                    meshMaterial = inputMaterial;
                }

                foreach (var v in mesh.Vertices)
                {
                    var vertex = new Vector3(v.X, v.Y, v.Z);
                    model.Vertices.Add(vertex);

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

                foreach (var normal in mesh.Normals)
                {
                    model.Normals.Add(new Vector3(normal.X, normal.Y, normal.Z));
                }

                foreach (var face in mesh.Faces)
                {
                    model.Triangles.Add(new Triangle()
                    {
                        A = model.Vertices[face.Indices[0]],
                        B = model.Vertices[face.Indices[1]],
                        C = model.Vertices[face.Indices[2]],
                        Normal = Vector3.Normalize(model.Normals[face.Indices[0]] + model.Normals[face.Indices[1]] + model.Normals[face.Indices[2]]),
                        SurfaceMaterial = meshMaterial
                    });
                }
            }

            model.BoundingBox = new AABB() { MinimumPoint = minPoint, MaximumPoint = maxPoint };

            importer.Dispose();
            return model;
        }
    }
}

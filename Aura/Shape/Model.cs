using Aura.Values;
using System.Collections.Generic;
using System.Numerics;

namespace Aura.Shape
{
    class Model : Primitive
    {

        public readonly List<Triangle> Triangles;

        public AABB BoundingBox { get; set; }

        //public override Matrix4x4 Transform
        //{
        //    get
        //    {
        //        return base.Transform;
        //    }

        //    set
        //    {
        //        base.Transform = value;

        //        var minPoint = new Vector3(float.PositiveInfinity);
        //        var maxPoint = new Vector3(float.NegativeInfinity);

        //        foreach (var triangle in Triangles)
        //        {
        //            triangle.Transform = Transform;
        //        }
        //    }
        //}

        public Model()
        {

            Triangles = new List<Triangle>();
        }

        public override Intersection Intersect(Ray ray)
        {
            if (BoundingBox.Intersect(ray) == null)
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

        public static Model LoadModel(string filename, Matrix4x4 transform, Material inputMaterial = null)
        {
            var importer = new Assimp.AssimpContext();

            Assimp.Scene scene = importer.ImportFile(filename, Assimp.PostProcessPreset.TargetRealTimeMaximumQuality);

            var model = new Model();

            var minPoint = new Vector3(float.PositiveInfinity);
            var maxPoint = new Vector3(float.NegativeInfinity);

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();

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

                    if (!transform.IsIdentity)
                    {
                        vertex = Vector3.Transform(vertex, transform);
                    }

                    vertices.Add(vertex);

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
                    var norm = new Vector3(normal.X, normal.Y, normal.Z);
                    if (!transform.IsIdentity)
                    {
                        norm = Vector3.TransformNormal(norm, transform);
                    }
                    normals.Add(norm);
                }

                foreach (var face in mesh.Faces)
                {
                    model.Triangles.Add(new Triangle()
                    {
                        A = vertices[face.Indices[0]],
                        B = vertices[face.Indices[1]],
                        C = vertices[face.Indices[2]],
                        Normal = Vector3.Normalize(normals[face.Indices[0]] + normals[face.Indices[1]] + normals[face.Indices[2]]),
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

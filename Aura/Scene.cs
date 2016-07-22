using Aura.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace Aura
{
    class Scene : IIntersectable
    {
        public int ImageWidth { get; private set; }

        public int ImageHeight { get; private set; }

        public Vector3 BackgroundColor { get; private set; }

        public float Exposure { get; private set; }

        public int RecursiveDepthLimit { get; set; }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraDirection { get; private set; }

        public Vector3 CameraUp { get; private set; }

        public float CameraFOV { get; private set; }

        public List<Primitive> SceneObject { get; set; }

        public Dictionary<string, Material> MaterialBank { get; set; }

        public void LoadScene(string sceneFile)
        {
            var sceneRoot = XElement.Load(sceneFile);

            ImageWidth = int.Parse(sceneRoot.Attribute("image_width").Value);
            ImageHeight = int.Parse(sceneRoot.Attribute("image_height").Value);
            BackgroundColor = sceneRoot.Attribute("background_color").Value.ToVec3();
            Exposure = float.Parse(sceneRoot.Attribute("exposure").Value);
            RecursiveDepthLimit = int.Parse(sceneRoot.Attribute("recursive_depth_limit").Value);

            var cameraNode = sceneRoot.Descendants().First(node => node.Name.LocalName.Equals("camera"));
            CameraPosition = cameraNode.Attribute("position").Value.ToVec3();
            CameraDirection = cameraNode.Attribute("direction").Value.ToVec3();
            CameraUp = cameraNode.Attribute("up").Value.ToVec3();
            CameraFOV = float.Parse(cameraNode.Attribute("fov").Value);

            var materials = sceneRoot.Descendants().First(node => node.Name.LocalName.Equals("materials"));
            MaterialBank = materials.Descendants()
                                    .Select(node => new
                                    {
                                        name = node.Name.LocalName,
                                        material = new Material()
                                        {
                                            Emission = node.Attribute("emission").Value.ToVec3(),
                                            Diffuse = node.Attribute("diffuse").Value.ToVec3(),
                                            Transparency = node.Attribute("transparency").Value.ToVec3(),
                                            RefractionIndex = float.Parse(node.Attribute("refraction_index").Value),
                                            Type = (Material.MaterialType)Enum.Parse(typeof(Material.MaterialType), node.Attribute("type").Value, ignoreCase: true)
                                        }
                                    }).ToDictionary(x => x.name, x => x.material);

            var objects = sceneRoot.Descendants().Where(element => element.Name.LocalName.Equals("object"));
            SceneObject = objects.Select(obj =>
            {
                var name = obj.Attribute("name")?.Value;
                Matrix4x4 transform = ParseTransform(obj.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals("transforms")));

                switch (obj.Attribute("type").Value)
                {
                    case "sphere":
                        return new Sphere()
                        {
                            Center = obj.Attribute("center").Value.ToVec3(),
                            Radius = double.Parse(obj.Attribute("radius").Value),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = name,
                            Transform = transform,
                        };
                    case "aabb":
                        return new AABB()
                        {
                            MinimumPoint = obj.Attribute("min_point").Value.ToVec3(),
                            MaximumPoint = obj.Attribute("max_point").Value.ToVec3(),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = name,
                            Transform = transform,
                        };
                    case "plane":
                        return new Shape.Plane()
                        {
                            Origin = obj.Attribute("origin").Value.ToVec3(),
                            Normal = obj.Attribute("normal").Value.ToVec3(),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            SurfaceMaterialSecondary = obj.Attribute("material_secondary") != null ? MaterialBank[obj.Attribute("material_secondary").Value] : null,
                            Name = name,
                            Transform = transform,
                        };
                    case "triangle":
                        return new Triangle()
                        {
                            A = obj.Attribute("a").Value.ToVec3(),
                            B = obj.Attribute("b").Value.ToVec3(),
                            C = obj.Attribute("c").Value.ToVec3(),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = name,
                            Transform = transform,
                        } as Primitive;
                    case "model":
                        var model = Model.LoadModel(obj.Attribute("file").Value, obj.Attribute("material") == null ? null : MaterialBank[obj.Attribute("material").Value]);
                        model.Transform = transform;
                        return model;
                    default:
                        throw new SceneDescriptionException($"Unrecognized object type {obj.Attribute("type").Value}");
                }
            }).ToList();
        }

        private Matrix4x4 ParseTransform(XElement transformNode)
        {
            var transform = Matrix4x4.Identity;
            if (transformNode == null)
            {
                return transform;
            }

            foreach (var t in transformNode.Descendants())
            {
                if (t.Name.LocalName.Equals("translate"))
                {
                    var translationMatrix = Matrix4x4.CreateTranslation(t.Value.ToVec3());
                    transform = Matrix4x4.Multiply(transform, translationMatrix);
                }
                else if (t.Name.LocalName.Equals("rotate"))
                {
                    var rotationVector = t.Value.ToVec3();
                    if (rotationVector.X != 0)
                    {
                        transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateRotationX(rotationVector.X.ToRadians()));
                    }
                    if (rotationVector.Y != 0)
                    {
                        transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateRotationY(rotationVector.Y.ToRadians()));
                    }
                    if (rotationVector.Z != 0)
                    {
                        transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateRotationZ(rotationVector.Z.ToRadians()));
                    }
                }
                else if (t.Name.LocalName.Equals("scale"))
                {
                    transform = Matrix4x4.Multiply(transform, Matrix4x4.CreateScale(t.Value.ToVec3()));
                }
                else
                {
                    throw new SceneDescriptionException($"Unrecognized transform type {t.Name.LocalName}");
                }
            }
            return transform;
        }

        public Intersection Intersect(Ray ray)
        {
            Intersection finalIntersection = null;

            // TODO add acceleration structure instead of iterating through everything
            foreach (var obj in SceneObject)
            {
                var tempIntersection = obj.Intersect(ray);
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

        internal class SceneDescriptionException : Exception
        {
            public SceneDescriptionException(string msg) : base(msg)
            {
            }
        }
    }
}

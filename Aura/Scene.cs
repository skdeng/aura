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

        public Dictionary<string, Matrix4x4> TransformBank { get; set; }
        
        public void LoadScene(string sceneFile)
        {
            var y = new Mesh();
            y.LoadOBJ("../../../Scene/test.obj");

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
                                            Type = (Material.MaterialType) Enum.Parse(typeof(Material.MaterialType), node.Attribute("type").Value, ignoreCase: true)
                                        }
                                    }).ToDictionary(x => x.name, x => x.material);

            var transforms = sceneRoot.Descendants().FirstOrDefault(node => node.Name.LocalName.Equals("transforms"));
            TransformBank = transforms?.Descendants()
                                        .Select(node => new
                                        {
                                            name = node.Name.LocalName,
                                            transform = node.Value.ToMat4()
                                        }).ToDictionary(x => x.name, x => x.transform);

            var objects = sceneRoot.Descendants().Where(element => element.Name.LocalName.Equals("object"));
            SceneObject = objects.Select(obj =>
            {
                var name = obj.Attribute("name")?.Value;
                Matrix4x4 transform = obj.Attribute("transform") == null ? Matrix4x4.Identity : TransformBank[obj.Attribute("transform").Value];
                if (transform.IsIdentity)
                {
                    var transformList = obj.Descendants().Where(node => node.Name.LocalName.Equals("transform")).Select(node => node.Value.ToMat4());
                    if (transformList.Any())
                    {
                        // Stack all transforms together, in order
                        foreach(var t in transformList)
                        {
                            Matrix4x4.Multiply(transform, t);
                        }
                    }
                }

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
                        } as Primitive;
                    case "aabb":
                        return new AABB()
                        {
                            MinimumPoint = obj.Attribute("min_point").Value.ToVec3(),
                            MaximumPoint = obj.Attribute("max_point").Value.ToVec3(),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = name,
                            Transform = transform,
                        } as Primitive;
                    case "plane":
                        return new Shape.Plane()
                        {
                            Origin = obj.Attribute("origin").Value.ToVec3(),
                            Normal = obj.Attribute("normal").Value.ToVec3(),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            SurfaceMaterialSecondary = obj.Attribute("material_secondary") != null ? MaterialBank[obj.Attribute("material_secondary").Value] : null,
                            Name = name,
                            Transform = transform,
                        } as Primitive;
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
                    default:
                        return null;
                }
            }).ToList();
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
    }
}

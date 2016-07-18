using Aura.Shape;
using Aura.VecMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Aura
{
    class Scene
    {
        public int ImageWidth { get; private set; }

        public int ImageHeight { get; private set; }

        public Vec3 BackgroundColor { get; private set; }
        
        public double Exposure { get; private set; }

        public int RecursiveDepthLimit { get; set; }

        public Vec3 CameraPosition { get; private set; }

        public Vec3 CameraDirection { get; private set; }

        public Vec3 CameraUp { get; private set; }

        public double CameraFOV { get; private set; }

        public List<Primitive> SceneObject { get; set; }

        public Dictionary<string, Material> MaterialBank { get; set; }
        
        public List<Primitive> EmissivePrimitives { get; set; }

        public void LoadScene(string sceneFile)
        {
            var sceneRoot = XElement.Load(sceneFile);

            ImageWidth = int.Parse(sceneRoot.Attribute("image_width").Value);
            ImageHeight = int.Parse(sceneRoot.Attribute("image_height").Value);
            BackgroundColor = new Vec3(sceneRoot.Attribute("background_color").Value);
            Exposure = double.Parse(sceneRoot.Attribute("exposure").Value);
            RecursiveDepthLimit = int.Parse(sceneRoot.Attribute("recursive_depth_limit").Value);

            var cameraNode = sceneRoot.Descendants().First(node => node.Name.LocalName.Equals("camera"));
            CameraPosition = new Vec3(cameraNode.Attribute("position").Value);
            CameraDirection = new Vec3(cameraNode.Attribute("direction").Value);
            CameraUp = new Vec3(cameraNode.Attribute("up").Value);
            CameraFOV = double.Parse(cameraNode.Attribute("fov").Value);

            var materials = sceneRoot.Descendants().First(node => node.Name.LocalName.Equals("materials"));
            MaterialBank = materials.Descendants()
                                    .Select(node => new
                                    {
                                        name = node.Name.LocalName,
                                        material = new Material()
                                        {
                                            Emission = new Vec3(node.Attribute("emission").Value),
                                            Diffuse = new Vec3(node.Attribute("diffuse").Value),
                                            Transparency = new Vec3(node.Attribute("transparency").Value),
                                            RefractionIndex = double.Parse(node.Attribute("refraction_index").Value),
                                            Type = (Material.MaterialType) Enum.Parse(typeof(Material.MaterialType), node.Attribute("type").Value, ignoreCase: true)
                                        }
                                    }).ToDictionary(x => x.name, x => x.material);

            var objects = sceneRoot.Descendants().Where(element => element.Name.LocalName.Equals("object"));
            SceneObject = objects.Select(obj =>
            {
                switch (obj.Attribute("type").Value)
                {
                    case "sphere":
                        return new Sphere()
                        {
                            Center = new Vec3(obj.Attribute("center").Value),
                            Radius = double.Parse(obj.Attribute("radius").Value),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = obj.Attribute("name")?.Value
                        } as Primitive;
                    case "aabb":
                        return new AABB()
                        {
                            MinimumPoint = new Vec3(obj.Attribute("min_point").Value),
                            MaximumPoint = new Vec3(obj.Attribute("max_point").Value),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = obj.Attribute("name")?.Value
                        } as Primitive;
                    case "plane":
                        return new Plane()
                        {
                            Origin = new Vec3(obj.Attribute("origin").Value),
                            Normal = new Vec3(obj.Attribute("normal").Value),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            SurfaceMaterialSecondary = obj.Attribute("material_secondary") != null ? MaterialBank[obj.Attribute("material_secondary").Value] : null,
                            Name = obj.Attribute("name")?.Value
                        } as Primitive;
                    default:
                        return null;
                }
            }).ToList();

            EmissivePrimitives = SceneObject.Where(obj => obj.SurfaceMaterial.Emission.Min() > 0).ToList();
        }

        public Intersection Intersect(Ray ray)
        {
            Intersection finalIntersection = new Intersection() { Intersect = false, T = double.PositiveInfinity };

            // TODO add acceleration structure instead of iterating through everything
            foreach (var obj in SceneObject)
            {
                var tempIntersection = obj.Intersect(ray);
                if (tempIntersection.Intersect)
                {
                    if (tempIntersection.T < finalIntersection.T)
                    {
                        finalIntersection = tempIntersection;
                    }
                }
            }

            return finalIntersection;
        }
    }
}

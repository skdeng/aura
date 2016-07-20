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
        
        public List<Primitive> EmissivePrimitives { get; set; }

        public void LoadScene(string sceneFile)
        {
            var sceneRoot = XElement.Load(sceneFile);

            ImageWidth = int.Parse(sceneRoot.Attribute("image_width").Value);
            ImageHeight = int.Parse(sceneRoot.Attribute("image_height").Value);
            BackgroundColor = sceneRoot.Attribute("background_color").Value.ToVec();
            Exposure = float.Parse(sceneRoot.Attribute("exposure").Value);
            RecursiveDepthLimit = int.Parse(sceneRoot.Attribute("recursive_depth_limit").Value);

            var cameraNode = sceneRoot.Descendants().First(node => node.Name.LocalName.Equals("camera"));
            CameraPosition = cameraNode.Attribute("position").Value.ToVec();
            CameraDirection = cameraNode.Attribute("direction").Value.ToVec();
            CameraUp = cameraNode.Attribute("up").Value.ToVec();
            CameraFOV = float.Parse(cameraNode.Attribute("fov").Value);

            var materials = sceneRoot.Descendants().First(node => node.Name.LocalName.Equals("materials"));
            MaterialBank = materials.Descendants()
                                    .Select(node => new
                                    {
                                        name = node.Name.LocalName,
                                        material = new Material()
                                        {
                                            Emission = node.Attribute("emission").Value.ToVec(),
                                            Diffuse = node.Attribute("diffuse").Value.ToVec(),
                                            Transparency = node.Attribute("transparency").Value.ToVec(),
                                            RefractionIndex = float.Parse(node.Attribute("refraction_index").Value),
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
                            Center = obj.Attribute("center").Value.ToVec(),
                            Radius = double.Parse(obj.Attribute("radius").Value),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = obj.Attribute("name")?.Value
                        } as Primitive;
                    case "aabb":
                        return new AABB()
                        {
                            MinimumPoint = obj.Attribute("min_point").Value.ToVec(),
                            MaximumPoint = obj.Attribute("max_point").Value.ToVec(),
                            SurfaceMaterial = MaterialBank[obj.Attribute("material").Value],
                            Name = obj.Attribute("name")?.Value
                        } as Primitive;
                    case "plane":
                        return new Shape.Plane()
                        {
                            Origin = obj.Attribute("origin").Value.ToVec(),
                            Normal = obj.Attribute("normal").Value.ToVec(),
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

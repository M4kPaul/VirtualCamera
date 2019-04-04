using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VirtualCamera
{
    public struct Polygon
    {
        public int ID;
        public Vector4[] vertices;
        public Vector3 normal;
    }

    class WorldData
    {
        public List<Polygon> Polygons { get; } = new List<Polygon>();
        public BSPTree BSPTree { get; }

        public WorldData(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                int id = 0;
                string line;
                var vertices = new List<Vector4>();
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string[] points = line.Split(' ');
                        switch (line[0])
                        {
                            case 'v':
                                vertices.Add(new Vector4(float.Parse(points[1]), float.Parse(points[2]), float.Parse(points[3]), 1.0F));
                                break;
                            case 'f':
                                var poly = new Polygon { ID = id++, vertices = new Vector4[points.Length - 1] };
                                for (int i = 0; i < points.Length - 1; i++)
                                {
                                    poly.vertices[i] = vertices[int.Parse(points[i + 1]) - 1];
                                }
                                poly.normal = Transformations.CalculateSurfaceNormal(poly);

                                Polygons.Add(poly);
                                break;
                        }
                    }
                }
            }

            BSPTree = new BSPTree(Polygons);
            Polygons = BSPTree.Polygons;
        }
    }
}

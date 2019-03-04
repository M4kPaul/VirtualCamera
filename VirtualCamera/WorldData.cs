using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VirtualCamera
{
    class WorldData
    {
        public List<Vector4[]> Mesh { get; } = new List<Vector4[]>();

        public WorldData()
        {
            using (StreamReader sr = new StreamReader(@"3dModels\world.obj"))
            {
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
                                var face = new Vector4[3];
                                face[0] = vertices[int.Parse(points[1]) - 1];
                                face[1] = vertices[int.Parse(points[2]) - 1];
                                face[2] = vertices[int.Parse(points[3]) - 1];
                                Mesh.Add(face);
                                break;
                        }
                    }
                }
            }
        }
    }
}

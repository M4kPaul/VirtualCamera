using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VirtualCamera
{
    struct Triangle
    {
        public Vector4[] Vertices;
    }

    struct Mesh
    {
        public List<Triangle> Triangles;
    }

    class WorldData
    {
        public Mesh mesh;
        private List<Vector4> vertices;

        public WorldData()
        {
            using (StreamReader sr = new StreamReader(@"C:\Users\Admin\Documents\cube.obj"))
            {
                string line;
                vertices = new List<Vector4>();
                mesh = new Mesh { Triangles = new List<Triangle>() };
                while ((line = sr.ReadLine()) != null)
                {
                    string[] points = line.Split(' ');
                    switch (line[0])
                    {
                        case 'v':
                            vertices.Add(new Vector4(float.Parse(points[1]), float.Parse(points[2]), float.Parse(points[3]), 1.0F));
                            break;
                        case 'f':
                            Triangle face = new Triangle { Vertices = new Vector4[3] };
                            face.Vertices[0] = vertices[int.Parse(points[1]) - 1];
                            face.Vertices[1] = vertices[int.Parse(points[2]) - 1];
                            face.Vertices[2] = vertices[int.Parse(points[3]) - 1];
                            mesh.Triangles.Add(face);
                            break;
                    }
                }
            }
        }
    }
}

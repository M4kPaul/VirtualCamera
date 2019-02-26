using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VirtualCamera
{
    struct Face
    {
        public Vector3[] Points;
    }

    struct Mesh
    {
        public List<Face> Tris;
    }

    class WorldData
    {
        public Mesh mesh;

        public WorldData()
        {
            using (StreamReader sr = new StreamReader("WorldPoints.txt"))
            {
                string line;
                mesh = new Mesh { Tris = new List<Face>() };
                while ((line = sr.ReadLine()) != null)
                {
                    if (line[0] != '#')
                    {
                        string[] points = line.Split(' ');
                        Face face = new Face { Points = new Vector3[3] };
                        face.Points[0] = new Vector3(float.Parse(points[0]), float.Parse(points[1]), float.Parse(points[2]));
                        face.Points[1] = new Vector3(float.Parse(points[3]), float.Parse(points[4]), float.Parse(points[5]));
                        face.Points[2] = new Vector3(float.Parse(points[6]), float.Parse(points[7]), float.Parse(points[8]));
                        mesh.Tris.Add(face);
                    }
                }
            }
        }
    }
}

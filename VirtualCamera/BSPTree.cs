using System.Collections.Generic;
using System.Numerics;

namespace VirtualCamera
{
    public class BSPTree
    {
        enum Position { InFront, Behind, Intersect };

        public class Node
        {
            public int PolyID { get; }
            public Node Front { get; set; }
            public Node Back { get; set; }

            public Node(int polyID)
            {
                PolyID = polyID;
            }
        }

        public Node Root { get; }
        public List<Polygon> Polygons { get; set; }
        public BSPTree(List<Polygon> polygons)
        {
            Polygons = new List<Polygon>(polygons);
            Root = GenerateTree(polygons);
        }

        private Node GenerateTree(List<Polygon> polygons)
        {
            if (polygons.Count == 0)
            {
                return null;
            }
            else
            {
                var polyRoot = new Node(polygons[0].ID);

                var frontPolygons = new List<Polygon>();
                var backPolygons = new List<Polygon>();
                var d = -Vector3.Dot(polygons[0].normal, Transformations.V4ToV3(polygons[0].vertices[0]));
                var plane = new Vector4(polygons[0].normal, d);
                for (int i = 1; i < polygons.Count; i++)
                {
                    var polyPos = CheckPosition(polygons[i], plane);
                    if (polyPos == Position.InFront)
                    {
                        frontPolygons.Add(polygons[i]);
                    }
                    else if (polyPos == Position.Behind)
                    {
                        backPolygons.Add(polygons[i]);
                    }
                    else
                    {
                        var d2 = -Vector3.Dot(polygons[i].normal, Transformations.V4ToV3(polygons[i].vertices[0]));

                        var v = Vector3.Cross(polygons[0].normal, polygons[i].normal);
                        var dot = Vector3.Dot(v, v);

                        var u1 = d2 * polygons[0].normal;
                        var u2 = -d * polygons[i].normal;
                        var p = Vector3.Cross(u1 + u2, v / dot);

                        int p1pos = -1, p2pos = -1;
                        Vector3 point1 = new Vector3(), point2 = new Vector3();
                        for (int j = 0; j < polygons[i].vertices.Length; j++)
                        {
                            var current = polygons[i].vertices[j];
                            var next = polygons[i].vertices[(j + 1) % polygons[i].vertices.Length];

                            if (Vector4.Dot(current, plane) * Vector4.Dot(next, plane) <= 0)
                            {
                                var u = Transformations.V4ToV3(next - current);
                                var a = Vector3.Cross(v, u);
                                var b = Vector3.Cross(Transformations.V4ToV3(next) - p, u);

                                var t = 0.0F;
                                if (a.X != 0)
                                {
                                    t = b.X / a.X;
                                }
                                else if (a.Y != 0)
                                {
                                    t = b.Y / a.Y;
                                }
                                else if (a.Z != 0)
                                {
                                    t = b.Z / a.Z;
                                }

                                if (p1pos == -1)
                                {
                                    point1 = p + (t * v);
                                    p1pos = j + 1;
                                }
                                else
                                {
                                    point2 = p + (t * v);
                                    p2pos = j + 1;
                                }
                            }
                        }

                        var poly1 = new Polygon { ID = Polygons.Count, vertices = new Vector4[p1pos + (polygons[i].vertices.Length - p2pos) + 2], normal = polygons[i].normal };
                        for (int j = 0, k = 0; k < p1pos; j++, k++) poly1.vertices[j] = polygons[i].vertices[k];
                        poly1.vertices[p1pos] = new Vector4(point1, 1.0F);
                        poly1.vertices[p1pos + 1] = new Vector4(point2, 1.0F);
                        for (int j = p1pos + 2, k = p2pos; k < polygons[i].vertices.Length; j++, k++) poly1.vertices[j] = polygons[i].vertices[k];
                        poly1.normal = Transformations.CalculateSurfaceNormal(poly1);
                        Polygons.Add(poly1);

                        var poly2 = new Polygon { ID = Polygons.Count, vertices = new Vector4[p2pos - p1pos + 2], normal = polygons[i].normal };
                        poly2.vertices[0] = new Vector4(point1, 1.0F);
                        for (int j = 1, k = p1pos; k < p2pos; j++, k++) poly2.vertices[j] = polygons[i].vertices[k];
                        poly2.vertices[p2pos - p1pos + 1] = new Vector4(point2, 1.0F);
                        Polygons.Add(poly2);

                        if (CheckPosition(poly1, plane) == Position.InFront)
                        {
                            frontPolygons.Add(poly1);
                            backPolygons.Add(poly2);
                        }
                        else
                        {
                            frontPolygons.Add(poly2);
                            backPolygons.Add(poly1);
                        }
                    }
                }

                polyRoot.Front = GenerateTree(frontPolygons);
                polyRoot.Back = GenerateTree(backPolygons);
                return polyRoot;
            }
        }

        private Position CheckPosition(Polygon poly, Vector4 plane)
        {
            var pos = 0;
            foreach (var vertex in poly.vertices)
            {
                var res = Vector4.Dot(vertex, plane);
                if (pos == 0 && (res < -0.5F || res > 0.5F)) pos = res > 0 ? 1 : -1;
                //else if ((res > 0.5F && pos < 0) || (res < -0.5F && pos > 0)) return Position.Intersect;
            }

            return pos == 1 ? Position.InFront : Position.Behind;
        }
    }
}

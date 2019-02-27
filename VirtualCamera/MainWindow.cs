using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace VirtualCamera
{
    public partial class MainWindow : Form
    {
        private WorldData worldData;
        private float fov;
        private float pitch;
        private float yaw;
        private float roll;
        private Vector3 camera;
        private Vector3 lookDir;
        private bool isSolid;

        public MainWindow()
        {
            worldData = new WorldData();
            Reset();

            InitializeComponent();
        }

        private void Reset()
        {
            fov = 90;
            pitch = 0.0F;
            yaw = 0.0F;
            roll = 0.0F;
            camera = new Vector3(0, 0, 0);
            lookDir = new Vector3(0, 0, 1);
            isSolid = false;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) camera = Vector3.Add(camera, lookDir);
            if (e.KeyCode == Keys.Down) camera = Vector3.Subtract(camera, lookDir);
            if (e.KeyCode == Keys.Left) camera.X += 0.5F;
            if (e.KeyCode == Keys.Right) camera.X -= 0.5F;
            if (e.KeyCode == Keys.Space) camera.Y += 0.5F;
            if (e.KeyCode == Keys.ControlKey) camera.Y -= 0.5F;

            if (e.KeyCode == Keys.W) pitch += 0.1F;
            if (e.KeyCode == Keys.S) pitch -= 0.1F;
            if (e.KeyCode == Keys.A) yaw -= 0.1F;
            if (e.KeyCode == Keys.D) yaw += 0.1F;
            if (e.KeyCode == Keys.Q) roll += 0.1F;
            if (e.KeyCode == Keys.E) roll -= 0.1F;

            if (e.KeyCode == Keys.OemMinus) fov += 1.0F;
            if (e.KeyCode == Keys.Oemplus) fov -= 1.0F;

            if (e.KeyCode == Keys.H) isSolid = isSolid ? false : true;
            if (e.KeyCode == Keys.R) Reset();

            Canvas.Refresh();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var graphics = Canvas.CreateGraphics();
            graphics.Clear(Color.Black);

            var fovRad = 1.0F / (float)Math.Tan(fov * 0.5F / 180.0F * Math.PI);
            var aspectRatio = Canvas.Height / (float)Canvas.Width;
            var near = 0.1F;
            var far = 1000.0F;
            var projMat = new Matrix4x4
            {
                M11 = aspectRatio * fovRad,
                M22 = fovRad,
                M33 = far / (far - near),
                M43 = (-far * near) / (far - near),
                M34 = 1.0F
            };
            
            var up = new Vector3(0, 1, 0);
            var target = new Vector3(0, 0, 1);
            var cameraRotMat = Matrix4x4.Multiply(Matrix4x4.Multiply(Transformations.RotateX(pitch), Transformations.RotateY(yaw)), Transformations.RotateZ(roll));
            lookDir = Transformations.V4ToV3(Transformations.MultiplyMatVect(cameraRotMat, new Vector4(target, 1.0F)));
            target = Vector3.Add(camera, lookDir);
            var cameraMat = Transformations.PointAt(camera, target, up);
            var viewMat = Transformations.QuickInverse(cameraMat);

            var worldTransMat = Transformations.Translate(0.0F, -5.0F, 15.0F);
            var projMesh = new Mesh { Triangles = new List<Triangle>() };
            foreach (var triangle in worldData.mesh.Triangles)
            {
                var transformedTriangle = new Triangle { Vertices = new Vector4[3] };
                transformedTriangle.Vertices[0] = Transformations.MultiplyMatVect(worldTransMat, triangle.Vertices[0]);
                transformedTriangle.Vertices[1] = Transformations.MultiplyMatVect(worldTransMat, triangle.Vertices[1]);
                transformedTriangle.Vertices[2] = Transformations.MultiplyMatVect(worldTransMat, triangle.Vertices[2]);

                if (isSolid)
                {
                    Vector3 point0 = Transformations.V4ToV3(transformedTriangle.Vertices[0]);
                    Vector3 line1 = Vector3.Subtract(Transformations.V4ToV3(transformedTriangle.Vertices[1]), point0);
                    Vector3 line2 = Vector3.Subtract(Transformations.V4ToV3(transformedTriangle.Vertices[2]), point0);
                    Vector3 normal = Vector3.Normalize(Vector3.Cross(line1, line2));

                    if (Vector3.Dot(normal, Vector3.Subtract(point0, camera)) >= 0.0F) continue;
                }

                var viewedTriangle = new Triangle { Vertices = new Vector4[3] };
                viewedTriangle.Vertices[0] = Transformations.MultiplyMatVect(viewMat, transformedTriangle.Vertices[0]);
                viewedTriangle.Vertices[1] = Transformations.MultiplyMatVect(viewMat, transformedTriangle.Vertices[1]);
                viewedTriangle.Vertices[2] = Transformations.MultiplyMatVect(viewMat, transformedTriangle.Vertices[2]);

                var projTri = new Triangle { Vertices = new Vector4[3] };
                projTri.Vertices[0] = Transformations.MultiplyMatVect(projMat, viewedTriangle.Vertices[0]);
                projTri.Vertices[1] = Transformations.MultiplyMatVect(projMat, viewedTriangle.Vertices[1]);
                projTri.Vertices[2] = Transformations.MultiplyMatVect(projMat, viewedTriangle.Vertices[2]);

                projTri.Vertices[0] = Vector4.Divide(projTri.Vertices[0], projTri.Vertices[0].W);
                projTri.Vertices[1] = Vector4.Divide(projTri.Vertices[1], projTri.Vertices[1].W);
                projTri.Vertices[2] = Vector4.Divide(projTri.Vertices[2], projTri.Vertices[2].W);

                projTri.Vertices[0].X *= -1.0F; projTri.Vertices[0].Y *= -1.0F;
                projTri.Vertices[1].X *= -1.0F; projTri.Vertices[1].Y *= -1.0F;
                projTri.Vertices[2].X *= -1.0F; projTri.Vertices[2].Y *= -1.0F;

                Vector4 offsetView = new Vector4(1.0F, 1.0F, 0, 0);
                projTri.Vertices[0] = Vector4.Add(projTri.Vertices[0], offsetView);
                projTri.Vertices[1] = Vector4.Add(projTri.Vertices[1], offsetView);
                projTri.Vertices[2] = Vector4.Add(projTri.Vertices[2], offsetView);
                projTri.Vertices[0].X *= 0.5f * Canvas.Width; projTri.Vertices[0].Y *= 0.5f * Canvas.Height;
                projTri.Vertices[1].X *= 0.5f * Canvas.Width; projTri.Vertices[1].Y *= 0.5f * Canvas.Height;
                projTri.Vertices[2].X *= 0.5f * Canvas.Width; projTri.Vertices[2].Y *= 0.5f * Canvas.Height;

                projMesh.Triangles.Add(projTri);
            }

            projMesh.Triangles.Sort(Transformations.CompareTriangles);
            var pen = new Pen(Color.White, 1);
            var brush = new SolidBrush(Color.Gray);
            foreach (var projTriangle in projMesh.Triangles)
            {
                PointF[] projPoints =
                {
                   new PointF(projTriangle.Vertices[0].X, projTriangle.Vertices[0].Y),
                   new PointF(projTriangle.Vertices[1].X, projTriangle.Vertices[1].Y),
                   new PointF(projTriangle.Vertices[2].X, projTriangle.Vertices[2].Y)
                };
                //graphics.DrawLine(pen, new PointF(projTriangle.Vertices[0].X, projTriangle.Vertices[0].Y), new PointF(projTriangle.Vertices[1].X, projTriangle.Vertices[1].Y));
                //graphics.DrawLine(pen, new PointF(projTriangle.Vertices[0].X, projTriangle.Vertices[0].Y), new PointF(projTriangle.Vertices[2].X, projTriangle.Vertices[2].Y));
                //graphics.DrawLine(pen, new PointF(projTriangle.Vertices[1].X, projTriangle.Vertices[1].Y), new PointF(projTriangle.Vertices[2].X, projTriangle.Vertices[2].Y));
                graphics.DrawPolygon(pen, projPoints);
                if (isSolid)
                {
                    graphics.FillPolygon(brush, projPoints);
                }
            }
        }
    }
}

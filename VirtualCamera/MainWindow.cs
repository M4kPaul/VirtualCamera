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
        private Matrix4x4 rotZXMat;

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
            rotZXMat = Matrix4x4.Multiply(Transformations.RotateZ(0), Transformations.RotateX(0));
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) camera = Vector3.Add(camera, lookDir);
            if (e.KeyCode == Keys.Down) camera = Vector3.Subtract(camera, lookDir);
            if (e.KeyCode == Keys.Left) camera.X += 0.5F;
            if (e.KeyCode == Keys.Right) camera.X -= 0.5F;
            if (e.KeyCode == Keys.U) camera.Y += 0.5F;
            if (e.KeyCode == Keys.J) camera.Y -= 0.5F;

            if (e.KeyCode == Keys.W) pitch -= 0.1F;
            if (e.KeyCode == Keys.S) pitch += 0.1F;
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
            var rotYMat = Transformations.RotateY(yaw);
            lookDir = Transformations.V4ToV3(Transformations.MultiplyMatVect(rotYMat, new Vector4(target, 1.0F)));
            target = Vector3.Add(camera, lookDir);
            var cameraMat = Transformations.PointAt(camera, target, up);
            var viewMat = Transformations.QuickInverse(cameraMat);

            var rotZMat = Transformations.RotateZ(roll);
            var rotXMat = Transformations.RotateX(pitch);
            rotZXMat = Matrix4x4.Multiply(rotZMat, rotXMat);
            var worldTransMat = Transformations.Translate(0.0F, -5.0F, 15.0F);
            worldTransMat = Matrix4x4.Multiply(rotZXMat, worldTransMat);
            var projMesh = new List<Vector4[]>();
            foreach (var triangle in worldData.Mesh)
            {
                var transformedTri = new Vector4[3];
                transformedTri[0] = Transformations.MultiplyMatVect(worldTransMat, triangle[0]);
                transformedTri[1] = Transformations.MultiplyMatVect(worldTransMat, triangle[1]);
                transformedTri[2] = Transformations.MultiplyMatVect(worldTransMat, triangle[2]);

                if (isSolid)
                {
                    Vector3 point0 = Transformations.V4ToV3(transformedTri[0]);
                    Vector3 line1 = Vector3.Subtract(Transformations.V4ToV3(transformedTri[1]), point0);
                    Vector3 line2 = Vector3.Subtract(Transformations.V4ToV3(transformedTri[2]), point0);
                    Vector3 normal = Vector3.Normalize(Vector3.Cross(line1, line2));

                    if (Vector3.Dot(normal, Vector3.Subtract(point0, camera)) >= 0.0F) continue;
                }

                var viewedTri = new Vector4[3];
                viewedTri[0] = Transformations.MultiplyMatVect(viewMat, transformedTri[0]);
                viewedTri[1] = Transformations.MultiplyMatVect(viewMat, transformedTri[1]);
                viewedTri[2] = Transformations.MultiplyMatVect(viewMat, transformedTri[2]);

                var projTri = new Vector4[3];
                projTri[0] = Transformations.MultiplyMatVect(projMat, viewedTri[0]);
                projTri[1] = Transformations.MultiplyMatVect(projMat, viewedTri[1]);
                projTri[2] = Transformations.MultiplyMatVect(projMat, viewedTri[2]);

                projTri[0] = Vector4.Divide(projTri[0], projTri[0].W);
                projTri[1] = Vector4.Divide(projTri[1], projTri[1].W);
                projTri[2] = Vector4.Divide(projTri[2], projTri[2].W);

                projTri[0].X *= -1.0F; projTri[0].Y *= -1.0F;
                projTri[1].X *= -1.0F; projTri[1].Y *= -1.0F;
                projTri[2].X *= -1.0F; projTri[2].Y *= -1.0F;

                Vector4 offsetView = new Vector4(1.0F, 1.0F, 0, 0);
                projTri[0] = Vector4.Add(projTri[0], offsetView);
                projTri[1] = Vector4.Add(projTri[1], offsetView);
                projTri[2] = Vector4.Add(projTri[2], offsetView);
                projTri[0].X *= 0.5f * Canvas.Width; projTri[0].Y *= 0.5f * Canvas.Height;
                projTri[1].X *= 0.5f * Canvas.Width; projTri[1].Y *= 0.5f * Canvas.Height;
                projTri[2].X *= 0.5f * Canvas.Width; projTri[2].Y *= 0.5f * Canvas.Height;

                projMesh.Add(projTri);
            }

            projMesh.Sort(Transformations.CompareTriangles);
            var pen = new Pen(Color.White, 1);
            var brush = new SolidBrush(Color.Gray);
            foreach (var projTri in projMesh)
            {
                PointF[] projPoints =
                {
                   new PointF(projTri[0].X, projTri[0].Y),
                   new PointF(projTri[1].X, projTri[1].Y),
                   new PointF(projTri[2].X, projTri[2].Y)
                };
                //graphics.DrawLine(pen, new PointF(projTri.Vertices[0].X, projTri.Vertices[0].Y), new PointF(projTri.Vertices[1].X, projTri.Vertices[1].Y));
                //graphics.DrawLine(pen, new PointF(projTri.Vertices[0].X, projTri.Vertices[0].Y), new PointF(projTri.Vertices[2].X, projTri.Vertices[2].Y));
                //graphics.DrawLine(pen, new PointF(projTri.Vertices[1].X, projTri.Vertices[1].Y), new PointF(projTri.Vertices[2].X, projTri.Vertices[2].Y));
                try
                {
                    graphics.DrawPolygon(pen, projPoints);
                    if (isSolid)
                    {
                        graphics.FillPolygon(brush, projPoints);
                    }
                }
                catch { }
            }
        }
    }
}

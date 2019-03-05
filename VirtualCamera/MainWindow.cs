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
        private bool isSolid;
        //private Matrix4x4 rotZXMat;

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
            isSolid = false;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) camera.Z -= 0.5F;
            if (e.KeyCode == Keys.Down) camera.Z += 0.5F;
            if (e.KeyCode == Keys.Left) camera.X -= 0.5F;
            if (e.KeyCode == Keys.Right) camera.X += 0.5F;
            if (e.KeyCode == Keys.U) camera.Y -= 0.5F;
            if (e.KeyCode == Keys.J) camera.Y += 0.5F;

            if (e.KeyCode == Keys.W) pitch += 0.1F;
            if (e.KeyCode == Keys.S) pitch -= 0.1F;
            if (e.KeyCode == Keys.A) yaw += 0.1F;
            if (e.KeyCode == Keys.D) yaw -= 0.1F;
            if (e.KeyCode == Keys.Q) roll -= 0.1F;
            if (e.KeyCode == Keys.E) roll += 0.1F;

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
                M11 = fovRad * aspectRatio,
                M22 = fovRad,
                M33 = far / (far - near),
                M34 = 1.0F,
                M43 = (-near * far) / (far - near)
            };

            var rotXMat = Transformations.RotateX(pitch);
            var rotYMat = Transformations.RotateY(yaw);
            //var rotZMat = Transformations.RotateZ(roll);
            var rotYXMat = Matrix4x4.Multiply(rotYMat, rotXMat);
            var worldTransMat = Transformations.Translate(camera.X, camera.Y - 5.0F, camera.Z + 15.0F);
            worldTransMat = Matrix4x4.Multiply(rotYXMat, worldTransMat);
            var projMesh = new List<Vector4[]>();
            foreach (var triangle in worldData.Mesh)
            {
                var transTri = new Vector4[3];
                transTri[0] = Vector4.Transform(triangle[0], worldTransMat);
                transTri[1] = Vector4.Transform(triangle[1], worldTransMat);
                transTri[2] = Vector4.Transform(triangle[2], worldTransMat);

                if (isSolid)
                {
                    Vector3 point0 = Transformations.V4ToV3(transTri[0]);
                    Vector3 line1 = Vector3.Subtract(Transformations.V4ToV3(transTri[1]), point0);
                    Vector3 line2 = Vector3.Subtract(Transformations.V4ToV3(transTri[2]), point0);
                    Vector3 normal = Vector3.Normalize(Vector3.Cross(line1, line2));

                    if (Vector3.Dot(normal, Vector3.Subtract(point0, camera)) >= 0.0F) continue;
                }

                var projTri = new Vector4[3];
                projTri[0] = Vector4.Transform(transTri[0], projMat);
                projTri[1] = Vector4.Transform(transTri[1], projMat);
                projTri[2] = Vector4.Transform(transTri[2], projMat);

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

            if (isSolid) projMesh.Sort(Transformations.CompareTriangles);
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
                //graphics.DrawLine(pen, new PointF(projTri[0].X, projTri[0].Y), new PointF(projTri[1].X, projTri[1].Y));
                //graphics.DrawLine(pen, new PointF(projTri[0].X, projTri[0].Y), new PointF(projTri[2].X, projTri[2].Y));
                //graphics.DrawLine(pen, new PointF(projTri[1].X, projTri[1].Y), new PointF(projTri[2].X, projTri[2].Y));
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

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
        private Vector3 rightDir;
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
            camera = new Vector3(0.0F, 5.0F, -15.0F);
            isSolid = false;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) camera += lookDir;
            if (e.KeyCode == Keys.Down) camera -= lookDir;
            if (e.KeyCode == Keys.Left) camera += rightDir;
            if (e.KeyCode == Keys.Right) camera -= rightDir;
            if (e.KeyCode == Keys.U) camera.Y += 0.5F;
            if (e.KeyCode == Keys.J) camera.Y -= 0.5F;

            if (e.KeyCode == Keys.W) pitch = (pitch + 5.0F) % 360.0F;
            if (e.KeyCode == Keys.S) pitch = (pitch - 5.0F) % 360.0F;
            if (e.KeyCode == Keys.A) yaw = (yaw + 5.0F) % 360.0F;
            if (e.KeyCode == Keys.D) yaw = (yaw - 5.0F) % 360.0F;
            if (e.KeyCode == Keys.Q) roll = (roll + 5.0F) % 360.0F;
            if (e.KeyCode == Keys.E) roll = (roll - 5.0F) % 360.0F;

            if (e.KeyCode == Keys.OemMinus && fov < 179.0F) fov += 1.0F;
            if (e.KeyCode == Keys.Oemplus && fov > 1.0F) fov -= 1.0F;

            if (e.KeyCode == Keys.H) isSolid = isSolid ? false : true;
            if (e.KeyCode == Keys.R) Reset();

            labelFoV.Text = $"FoV: {fov}";
            labelX.Text = $"X: {camera.X}";
            labelY.Text = $"Y: {camera.Y}";
            labelZ.Text = $"Z: {camera.Z}";
            labelRX.Text = $"XR: {-pitch}";
            labelRY.Text = $"YR: {-yaw}";
            labelRZ.Text = $"ZR: {-roll}";

            Canvas.Refresh();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var graphics = Canvas.CreateGraphics();
            graphics.Clear(Color.Black);

            var fovRad = 1.0F / (float)Math.Tan(fov * 0.5F / 180.0F * Math.PI);
            var aspectRatio = Canvas.Height / (float)Canvas.Width;
            var near = 0.0F;
            var far = 1.0F;
            var projMat = new Matrix4x4
            {
                M11 = fovRad * aspectRatio,
                M22 = fovRad,
                M33 = far / (far - near),
                M34 = 1.0F,
                M43 = -near * far / (far - near)
            };

            var up = new Vector3(0.0F, 1.0F, 0.0F);
            var at = new Vector3(0.0F, 0.0F, 1.0F);
            var rotXMat = Transformations.RotateX(pitch / 180.0F * (float)Math.PI);
            var rotYMat = Transformations.RotateY(yaw / 180.0F * (float)Math.PI);
            var rotZMat = Transformations.RotateZ(roll / 180.0F * (float)Math.PI);
            var rotXYMat = rotXMat * rotYMat;
            lookDir = Transformations.V4ToV3(Vector4.Transform(new Vector4(at, 1.0F), rotXYMat));
            rightDir = Vector3.Normalize(Vector3.Cross(up, lookDir));
            at = camera + lookDir;
            var viewMat = Transformations.LookAt(camera, at, up) * rotZMat;

            var projMesh = new List<Vector4[]>();
            foreach (var triangle in worldData.Mesh)
            {
                if (isSolid)
                {
                    Vector3 point0 = Transformations.V4ToV3(triangle[0]);
                    Vector3 line1 = Transformations.V4ToV3(triangle[1]) - point0;
                    Vector3 line2 = Transformations.V4ToV3(triangle[2]) - point0;
                    Vector3 normal = Vector3.Normalize(Vector3.Cross(line1, line2));

                    if (Vector3.Dot(normal, point0 - camera) >= 0.0F) continue;
                }

                var viewTri = new Vector4[3];
                viewTri[0] = Vector4.Transform(triangle[0], viewMat);
                viewTri[1] = Vector4.Transform(triangle[1], viewMat);
                viewTri[2] = Vector4.Transform(triangle[2], viewMat);

                var projTri = new Vector4[3];
                projTri[0] = Vector4.Transform(viewTri[0], projMat);
                projTri[1] = Vector4.Transform(viewTri[1], projMat);
                projTri[2] = Vector4.Transform(viewTri[2], projMat);

                if (projTri[0].X < -projTri[0].W || projTri[0].X > projTri[0].W ||
                    projTri[0].Y < -projTri[0].W || projTri[0].Y > projTri[0].W ||
                    projTri[0].Z < 0 || projTri[0].Z > projTri[0].W ||
                    projTri[1].X < -projTri[1].W || projTri[1].X > projTri[1].W ||
                    projTri[1].Y < -projTri[1].W || projTri[1].Y > projTri[1].W ||
                    projTri[1].Z < 0 || projTri[1].Z > projTri[1].W ||
                    projTri[2].X < -projTri[2].W || projTri[2].X > projTri[2].W ||
                    projTri[2].Y < -projTri[2].W || projTri[2].Y > projTri[2].W ||
                    projTri[2].Z < 0 || projTri[2].Z > projTri[2].W) continue;

                projTri[0] /= projTri[0].W;
                projTri[1] /= projTri[1].W;
                projTri[2] /= projTri[2].W;

                projTri[0].X *= -1.0F; projTri[0].Y *= -1.0F;
                projTri[1].X *= -1.0F; projTri[1].Y *= -1.0F;
                projTri[2].X *= -1.0F; projTri[2].Y *= -1.0F;

                Vector4 offsetView = new Vector4(1.0F, 1.0F, 0, 0);
                projTri[0] += offsetView;
                projTri[1] += offsetView;
                projTri[2] += offsetView;
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

                graphics.DrawPolygon(pen, projPoints);
                if (isSolid) graphics.FillPolygon(brush, projPoints);
            }
        }
    }
}

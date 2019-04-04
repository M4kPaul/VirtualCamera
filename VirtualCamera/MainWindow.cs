using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace VirtualCamera
{
    public partial class MainWindow : Form
    {
        Graphics graphics;
        private readonly WorldData worldData;
        private List<Polygon> projPolygons;
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
            camera = new Vector3(0.0F, 0.0F, 0.0F);
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
            graphics = Canvas.CreateGraphics();
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
                M43 = -near * far / (far - near)
            };

            var up = new Vector3(0.0F, 1.0F, 0.0F);
            var at = new Vector3(0.0F, 0.0F, -1.0F);
            var rotXMat = Transformations.RotateX(pitch / 180.0F * (float)Math.PI);
            var rotYMat = Transformations.RotateY(yaw / 180.0F * (float)Math.PI);
            var rotZMat = Transformations.RotateZ(roll / 180.0F * (float)Math.PI);
            var rotXYMat = rotXMat * rotYMat;
            lookDir = Transformations.V4ToV3(Vector4.Transform(new Vector4(at, 1.0F), rotXYMat));
            rightDir = Vector3.Normalize(Vector3.Cross(up, lookDir));
            at = camera + lookDir;
            var viewMat = Transformations.LookAt(camera, at, up) * rotZMat;

            projPolygons = new List<Polygon>();
            foreach (var poly in worldData.Polygons)
            {
                bool isClipped = false;
                var viewedPoly = new Vector4();
                var projPoly = new Polygon { ID = poly.ID, vertices = new Vector4[poly.vertices.Length] };
                for (int i = 0; i <  poly.vertices.Length; i++)
                {
                    viewedPoly = Vector4.Transform(poly.vertices[i], viewMat);
                    projPoly.vertices[i] = Vector4.Transform(viewedPoly, projMat);

                    // TODO: fix clipping
                    if (projPoly.vertices[i].X < -projPoly.vertices[i].W || projPoly.vertices[i].X > projPoly.vertices[i].W ||
                        projPoly.vertices[i].Y < -projPoly.vertices[i].W || projPoly.vertices[i].Y > projPoly.vertices[i].W ||
                        projPoly.vertices[i].Z < 0 || projPoly.vertices[i].Z > projPoly.vertices[i].W) isClipped = true;

                    projPoly.vertices[i] /= projPoly.vertices[i].W;
                    projPoly.vertices[i].X *= -1.0F; projPoly.vertices[i].Y *= -1.0F;
                    projPoly.vertices[i] += new Vector4(1.0F, 1.0F, 0, 0);
                    projPoly.vertices[i].X *= 0.5f * Canvas.Width; projPoly.vertices[i].Y *= 0.5f * Canvas.Height;
                }

                if (!isClipped) projPolygons.Add(projPoly);
            }

            if (projPolygons.Count > 0) TraverseProjection(worldData.BSPTree.Root);
        }

        private void TraverseProjection(BSPTree.Node Node)
        {
            var plane = worldData.Polygons.Find(p => p.ID == Node.PolyID);
            bool isCameraInFront = Vector3.Dot(plane.normal, camera - Transformations.V4ToV3(plane.vertices[0])) > 0.0F ? true : false;
            if (isCameraInFront)
            {
                if (Node.Back != null) TraverseProjection(Node.Back);
                PaintNode(Node.PolyID);
                if (Node.Front != null) TraverseProjection(Node.Front);
            }
            else
            {
                if (Node.Front != null) TraverseProjection(Node.Front);
                PaintNode(Node.PolyID);
                if (Node.Back != null) TraverseProjection(Node.Back);
            }
        }

        private void PaintNode(int ID)
        {
            var pen = new Pen(Color.White, 1);
            var brush = new SolidBrush(Color.Gray);

            var projPolygon = projPolygons.Find(p => p.ID == ID);
            if (projPolygon.vertices != null)
            {
                var projPoints = new PointF[projPolygon.vertices.Length];
                for (int i = 0; i < projPolygon.vertices.Length; i++)
                {
                    projPoints[i] = new PointF(projPolygon.vertices[i].X, projPolygon.vertices[i].Y);
                }

                graphics.DrawPolygon(pen, projPoints);
                if (isSolid) graphics.FillPolygon(brush, projPoints);
            }
        }
    }
}

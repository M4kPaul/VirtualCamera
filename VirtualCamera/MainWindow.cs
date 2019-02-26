using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace VirtualCamera
{
    public partial class MainWindow : Form
    {
        private WorldData worldData;
        private float near;
        private float far;
        private float fov;
        private float fovRad;
        private float aspectRatio;
        private Matrix4x4 projMat;
        private float frame;
        private bool isSolid;

        public MainWindow()
        {
            worldData = new WorldData();
            near = 0.1F;
            far = 1000.0F;
            fov = 90.0F;
            frame = 0.0F;
            isSolid = false;

            InitializeComponent();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) frame += 0.1F;
            if (e.KeyCode == Keys.Down) frame -= 0.1F;
            if (e.KeyCode == Keys.Left) frame += 0.1F;
            if (e.KeyCode == Keys.Right) frame += 0.1F;

            if (e.KeyCode == Keys.W) frame += 0.1F;
            if (e.KeyCode == Keys.S) frame += 0.1F;
            if (e.KeyCode == Keys.A) frame += 0.1F;
            if (e.KeyCode == Keys.D) frame += 0.1F;

            if (e.KeyCode == Keys.OemMinus) fov -= 1.0F;
            if (e.KeyCode == Keys.Oemplus) fov += 1.0F;

            if (e.KeyCode == Keys.H) isSolid = isSolid ? false : true;

            Canvas.Refresh();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var graphics = Canvas.CreateGraphics();
            graphics.Clear(Color.Black);

            fovRad = 1.0F / (float)Math.Tan(fov * 0.5F / 180.0F * Math.PI);
            aspectRatio = Canvas.Height / (float)Canvas.Width;
            projMat = new Matrix4x4
            {
                M11 = aspectRatio * fovRad,
                M22 = fovRad,
                M33 = far / (far - near),
                M43 = (-far * near) / (far - near),
                M34 = 1.0F
            };

            Matrix4x4 rotZMat = Transformations.RotateZ(frame * 0.5F);
            Matrix4x4 rotXMat = Transformations.RotateX(frame);

            Matrix4x4 transMat = Transformations.Translate(0.0F, 0.0F, 5.0F);
            Matrix4x4 worldMat = Matrix4x4.Multiply(rotZMat, rotXMat);
            //worldMat = Matrix4x4.Multiply(worldMat, transMat);
            //projMat = Matrix4x4.Multiply(projMat, worldMat);

            var pen = new Pen(Color.White, 1);
            foreach (var triangle in worldData.mesh.Triangles)
            {
                Triangle projTriangle = new Triangle { Vertices = new Vector4[3] };
                projTriangle.Vertices[0] = Transformations.MultiplyMatVect(worldMat, triangle.Vertices[0]);
                projTriangle.Vertices[1] = Transformations.MultiplyMatVect(worldMat, triangle.Vertices[1]);
                projTriangle.Vertices[2] = Transformations.MultiplyMatVect(worldMat, triangle.Vertices[2]);

                projTriangle.Vertices[0].Z = projTriangle.Vertices[0].Z + 3.0F;
                projTriangle.Vertices[1].Z = projTriangle.Vertices[1].Z + 3.0F;
                projTriangle.Vertices[2].Z = projTriangle.Vertices[2].Z + 3.0F;

                projTriangle.Vertices[0] = Transformations.MultiplyMatVect(projMat, projTriangle.Vertices[0]);
                projTriangle.Vertices[1] = Transformations.MultiplyMatVect(projMat, projTriangle.Vertices[1]);
                projTriangle.Vertices[2] = Transformations.MultiplyMatVect(projMat, projTriangle.Vertices[2]);

                if (projTriangle.Vertices[0].W != 0)
                    projTriangle.Vertices[0] = Vector4.Divide(projTriangle.Vertices[0], projTriangle.Vertices[0].W);
                if (projTriangle.Vertices[1].W != 0)
                    projTriangle.Vertices[1] = Vector4.Divide(projTriangle.Vertices[1], projTriangle.Vertices[1].W);
                if (projTriangle.Vertices[2].W != 0)
                    projTriangle.Vertices[2] = Vector4.Divide(projTriangle.Vertices[2], projTriangle.Vertices[2].W);

                projTriangle.Vertices[0].X += 1.0f; projTriangle.Vertices[0].Y += 1.0f;
                projTriangle.Vertices[1].X += 1.0f; projTriangle.Vertices[1].Y += 1.0f;
                projTriangle.Vertices[2].X += 1.0f; projTriangle.Vertices[2].Y += 1.0f;
                projTriangle.Vertices[0].X *= 0.5f * Canvas.Width; projTriangle.Vertices[0].Y *= 0.5f * Canvas.Height;
                projTriangle.Vertices[1].X *= 0.5f * Canvas.Width; projTriangle.Vertices[1].Y *= 0.5f * Canvas.Height;
                projTriangle.Vertices[2].X *= 0.5f * Canvas.Width; projTriangle.Vertices[2].Y *= 0.5f * Canvas.Height;

                graphics.DrawLine(pen, new PointF(projTriangle.Vertices[0].X, projTriangle.Vertices[0].Y), new PointF(projTriangle.Vertices[1].X, projTriangle.Vertices[1].Y));
                graphics.DrawLine(pen, new PointF(projTriangle.Vertices[0].X, projTriangle.Vertices[0].Y), new PointF(projTriangle.Vertices[2].X, projTriangle.Vertices[2].Y));
                graphics.DrawLine(pen, new PointF(projTriangle.Vertices[1].X, projTriangle.Vertices[1].Y), new PointF(projTriangle.Vertices[2].X, projTriangle.Vertices[2].Y));
            }

            //Thread.Sleep(10);
            //frame += 0.03F;
            //Canvas.Refresh();
        }
    }
}

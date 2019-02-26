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
        private float AspectRatio;
        private Matrix4x4 projMat;
        private float frame;
        private Matrix4x4 rotXMat;
        private Matrix4x4 rotZMat;

        public MainWindow()
        {
            worldData = new WorldData();
            near = 0.1F;
            far = 1000.0F;
            fov = 90.0F;
            frame = 0.0F;

            InitializeComponent();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var graphics = Canvas.CreateGraphics();
            graphics.Clear(Color.Black);

            fovRad = 1.0F / (float)Math.Tan(fov * 0.5F / 180.0F * Math.PI);
            AspectRatio = Canvas.Height / (float)Canvas.Width;
            rotXMat = new Matrix4x4(
                    (float)Math.Cos(frame), (float)Math.Sin(frame), 0, 0,
                    -(float)Math.Sin(frame), (float)Math.Cos(frame), 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                );
            rotZMat = new Matrix4x4(
                    1, 0, 0, 0,
                    0, (float)Math.Cos(frame * 0.5F), (float)Math.Sin(frame * 0.5F), 0,
                    0, -(float)Math.Sin(frame * 0.5f), (float)Math.Cos(frame * 0.5F), 0,
                    0, 0, 0, 1
                );
            projMat = new Matrix4x4(
                    AspectRatio * fovRad, 0, 0, 0,
                    0, fovRad, 0, 0,
                    0, 0, far / (far - near), 1.0F,
                    0, 0, (-far * near) / (far - near), 0
                );

            var pen = new Pen(Color.White, 1);
            foreach (var face in worldData.mesh.Tris)
            {
                Face rotZFace = new Face { Points = new Vector3[3] };
                MultiplyMatrixVector(face.Points[0], out rotZFace.Points[0], rotZMat);
                MultiplyMatrixVector(face.Points[1], out rotZFace.Points[1], rotZMat);
                MultiplyMatrixVector(face.Points[2], out rotZFace.Points[2], rotZMat);

                Face rotZXFace = new Face { Points = new Vector3[3] };
                MultiplyMatrixVector(rotZFace.Points[0], out rotZXFace.Points[0], rotXMat);
                MultiplyMatrixVector(rotZFace.Points[1], out rotZXFace.Points[1], rotXMat);
                MultiplyMatrixVector(rotZFace.Points[2], out rotZXFace.Points[2], rotXMat);

                rotZXFace.Points[0].Z += 3.0F;
                rotZXFace.Points[1].Z += 3.0F;
                rotZXFace.Points[2].Z += 3.0F;

                Face projFace = new Face { Points = new Vector3[3] };
                MultiplyMatrixVector(rotZXFace.Points[0], out projFace.Points[0], projMat);
                MultiplyMatrixVector(rotZXFace.Points[1], out projFace.Points[1], projMat);
                MultiplyMatrixVector(rotZXFace.Points[2], out projFace.Points[2], projMat);

                projFace.Points[0].X += 1.0F; projFace.Points[0].Y += 1.0F;
                projFace.Points[1].X += 1.0F; projFace.Points[1].Y += 1.0F;
                projFace.Points[2].X += 1.0F; projFace.Points[2].Y += 1.0F;

                projFace.Points[0].X *= 0.5F * Canvas.Width; projFace.Points[0].Y *= 0.5F * Canvas.Height;
                projFace.Points[1].X *= 0.5F * Canvas.Width; projFace.Points[1].Y *= 0.5F * Canvas.Height;
                projFace.Points[2].X *= 0.5F * Canvas.Width; projFace.Points[2].Y *= 0.5F * Canvas.Height;

                graphics.DrawLine(pen, new PointF(projFace.Points[0].X, projFace.Points[0].Y), new PointF(projFace.Points[1].X, projFace.Points[1].Y));
                graphics.DrawLine(pen, new PointF(projFace.Points[0].X, projFace.Points[0].Y), new PointF(projFace.Points[2].X, projFace.Points[2].Y));
                graphics.DrawLine(pen, new PointF(projFace.Points[1].X, projFace.Points[1].Y), new PointF(projFace.Points[2].X, projFace.Points[2].Y));
            }

            Thread.Sleep(10);
            frame += 0.03F;
            Canvas.Refresh();
        }

        private void MultiplyMatrixVector(Vector3 point, out Vector3 projPoint, Matrix4x4 mat)
        {
            projPoint.X = point.X * mat.M11 + point.Y * mat.M21 + point.Z * mat.M31 + mat.M41;
            projPoint.Y = point.X * mat.M12 + point.Y * mat.M22 + point.Z * mat.M32 + mat.M42;
            projPoint.Z = point.X * mat.M13 + point.Y * mat.M23 + point.Z * mat.M33 + mat.M43;
            float w = point.X * mat.M14 + point.Y * mat.M24 + point.Z * mat.M34 + mat.M44;

            if (w != 0.0F)
            {
                projPoint.X /= w;
                projPoint.Y /= w;
                projPoint.Z /= w;
            }
        }
    }
}

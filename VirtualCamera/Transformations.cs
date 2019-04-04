using System;
using System.Numerics;

namespace VirtualCamera
{
    static class Transformations
    {
        public static Matrix4x4 Translate(float x, float y, float z)
        {
            Matrix4x4 res = new Matrix4x4
            {
                M11 = 1.0F,
                M22 = 1.0F,
                M33 = 1.0F,
                M41 = x,
                M42 = y,
                M43 = z,
                M44 = 1.0F,
            };

            return res;
        }

        public static Matrix4x4 RotateX(float radAngle)
        {
            Matrix4x4 res = new Matrix4x4
            {
                M11 = 1.0F,
                M22 = (float)Math.Cos(radAngle),
                M23 = (float)Math.Sin(radAngle),
                M32 = -(float)Math.Sin(radAngle),
                M33 = (float)Math.Cos(radAngle),
                M44 = 1.0F
            };

            return res;
        }

        public static Matrix4x4 RotateY(float radAngle)
        {
            Matrix4x4 res = new Matrix4x4
            {
                M11 = (float)Math.Cos(radAngle),
                M13 = -(float)Math.Sin(radAngle),
                M22 = 1.0F,
                M31 = (float)Math.Sin(radAngle),
                M33 = (float)Math.Cos(radAngle),
                M44 = 1.0F
            };

            return res;
        }

        public static Matrix4x4 RotateZ(float radAngle)
        {
            Matrix4x4 res = new Matrix4x4
            {
                M11 = (float)Math.Cos(radAngle),
                M12 = (float)Math.Sin(radAngle),
                M21 = -(float)Math.Sin(radAngle),
                M22 = (float)Math.Cos(radAngle),
                M33 = 1.0F,
                M44 = 1.0F
            };

            return res;
        }

        public static Matrix4x4 LookAt(Vector3 camera, Vector3 at, Vector3 up)
        {
            var zAxis = Vector3.Normalize(at - camera);
            var xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
            var yAxis = Vector3.Cross(zAxis, xAxis);

            Matrix4x4 res = new Matrix4x4(
                xAxis.X, yAxis.X, zAxis.X, 0.0F,
                xAxis.Y, yAxis.Y, zAxis.Y, 0.0F,
                xAxis.Z, yAxis.Z, zAxis.Z, 0.0F,
                -Vector3.Dot(xAxis, camera), -Vector3.Dot(yAxis, camera), -Vector3.Dot(zAxis, camera), 1.0F
            );

            return res;
        }

        public static Vector3 V4ToV3(Vector4 v)
        {
            Vector3 res = new Vector3
            {
                X = v.X,
                Y = v.Y,
                Z = v.Z
            };

            return res;
        }

        public static Vector3 CalculateSurfaceNormal(Polygon polygon)
        {
            var normal = new Vector3(0, 0, 0);

            for (int i = 0; i < polygon.vertices.Length; i++)
            {
                var current = polygon.vertices[i];
                var next = polygon.vertices[(i + 1) % polygon.vertices.Length]; 
                normal.X += (current.Y - next.Y) * (current.Z + next.Z);
                normal.Y += (current.Z - next.Z) * (current.X + next.X);
                normal.Z += (current.X - next.X) * (current.Y + next.Y);
            }

            return Vector3.Normalize(normal);
        }
    }
}

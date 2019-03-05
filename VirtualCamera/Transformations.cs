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
                M23 = -(float)Math.Sin(radAngle),
                M32 = (float)Math.Sin(radAngle),
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
                M13 = (float)Math.Sin(radAngle),
                M22 = 1.0F,
                M31 = -(float)Math.Sin(radAngle),
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
                M12 = -(float)Math.Sin(radAngle),
                M21 = (float)Math.Sin(radAngle),
                M22 = (float)Math.Cos(radAngle),
                M33 = 1.0F,
                M44 = 1.0F
            };

            return res;
        }

        public static int CompareTriangles(Vector4[] t1, Vector4[] t2)
        {
            float z1 = (t1[0].Z + t1[1].Z + t1[2].Z) / 3.0F;
            float z2 = (t2[0].Z + t2[1].Z + t2[2].Z) / 3.0F;
            return z2.CompareTo(z1);
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
    }
}

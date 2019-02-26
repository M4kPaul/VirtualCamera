using System;
using System.Numerics;

namespace VirtualCamera
{
    static class Transformations
    {
        public static Vector4 MultiplyMatVect(Matrix4x4 mat, Vector4 vect)
        {
            Vector4 res = new Vector4
            {
                X = vect.X * mat.M11 + vect.Y * mat.M21 + vect.Z * mat.M31 + vect.W * mat.M41,
                Y = vect.X * mat.M12 + vect.Y * mat.M22 + vect.Z * mat.M32 + vect.W * mat.M42,
                Z = vect.X * mat.M13 + vect.Y * mat.M23 + vect.Z * mat.M33 + vect.W * mat.M43,
                W = vect.X * mat.M14 + vect.Y * mat.M24 + vect.Z * mat.M34 + vect.W * mat.M44
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
                M12 = (float)Math.Sin(radAngle),
                M21 = -(float)Math.Sin(radAngle),
                M22 = (float)Math.Cos(radAngle),
                M33 = 1.0F,
                M44 = 1.0F
            };

            return res;
        }

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
    }
}

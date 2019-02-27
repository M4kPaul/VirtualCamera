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

        public static int CompareTriangles(Triangle t1, Triangle t2)
        {
            float z1 = (t1.Vertices[0].Z + t1.Vertices[1].Z + t1.Vertices[2].Z) / 3.0F;
            float z2 = (t2.Vertices[0].Z + t2.Vertices[1].Z + t2.Vertices[2].Z) / 3.0F;
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

        public static Matrix4x4 PointAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            Vector3 newForward = Vector3.Normalize(Vector3.Subtract(target, pos));
            Vector3 newUp = Vector3.Normalize(Vector3.Subtract(up, Vector3.Multiply(newForward, Vector3.Dot(up, newForward))));
            Vector3 newRight = Vector3.Cross(newUp, newForward);

            Matrix4x4 res = new Matrix4x4(
                      newRight.X,   newRight.Y,   newRight.Z, 0.0F,
                         newUp.X,      newUp.Y,      newUp.Z, 0.0F,
                    newForward.X, newForward.Y, newForward.Z, 0.0F,
                           pos.X,        pos.Y,        pos.Z, 1.0F
                );

            return res;
        }
        
        public static Matrix4x4 QuickInverse(Matrix4x4 m) // trans/rot only!
        {
            Matrix4x4 res = new Matrix4x4(
                    m.M11, m.M21, m.M31, 0.0F,
                    m.M12, m.M22, m.M32, 0.0F,
                    m.M13, m.M23, m.M33, 0.0F,
                    -(m.M41 * m.M11 + m.M42 * m.M21 + m.M43 * m.M31),
                    -(m.M41 * m.M12 + m.M42 * m.M22 + m.M43 * m.M32),
                    -(m.M41 * m.M13 + m.M42 * m.M23 + m.M43 * m.M33),
                    1.0F
                );

            return res;
        }
    }
}

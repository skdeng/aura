using System;
using System.Numerics;

namespace Aura
{
    static class ExtensionMethods
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        
        public static Vector3 ToVec3(this string s)
        {
            var splitted = s.Split(' ');
            return new Vector3(float.Parse(splitted[0]), float.Parse(splitted[1]), float.Parse(splitted[2]));
        }

        public static Matrix4x4 ToMat4(this string s)
        {
            float[][] data = new float[4][];
            var rows = s.Split(';');
            for (int i = 0; i < 4; i++)
            {
                data[i] = new float[4];
                var e = rows[i].Split(' ');
                for (int j = 0; j < 4; j++)
                {
                    data[i][j] = float.Parse(e[j]);
                }
            }

            return new Matrix4x4
                (
                data[0][0], data[0][1], data[0][2], data[0][3],
                data[1][0], data[1][1], data[1][2], data[1][3],
                data[2][0], data[2][1], data[2][2], data[2][3],
                data[3][0], data[3][1], data[3][2], data[3][3]
                );
        }

        public static float Min(this Vector3 val)
        {
            return Math.Min(Math.Min(val.X, val.Y), val.Z);
        }

        public static float Max(this Vector3 val)
        {
            return Math.Max(Math.Max(val.X, val.Y), val.Z);
        }

        public static Vector3 Clamp(this Vector3 val, float min, float max)
        {
            return new Vector3(val.X.Clamp(min, max), val.Y.Clamp(min, max), val.Z.Clamp(min, max));
        }

        public static Vector3 Copy(this Vector3 val)
        {
            return new Vector3(val.X, val.Y, val.Z);
        }
    }
}

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
        
        public static Vector3 ToVec(this string s)
        {
            var splitted = s.Split(' ');
            return new Vector3(float.Parse(splitted[0]), float.Parse(splitted[1]), float.Parse(splitted[2]));
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

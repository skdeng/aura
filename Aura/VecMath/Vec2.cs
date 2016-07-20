using System;

namespace Aura.VecMath
{
    class Vec2 : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Length { get { return Math.Sqrt(X * X + Y * Y); } }
        public double LengthSq { get { return X * X + Y * Y; } }

        public Vec2()
        {
            X = Y = 0;
        }

        public Vec2(double value)
        {
            X = Y = value;
        }

        public Vec2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2(string s)
        {
            var splitted = s.Split(' ');
            X = double.Parse(splitted[0]);
            Y = double.Parse(splitted[1]);
        }

        public static Vec2 operator -(Vec2 u)
        {
            return new Vec2(-u.X, -u.Y);
        }

        public static Vec2 operator +(Vec2 u, Vec2 v)
        {
            return new Vec2(u.X + v.X, u.Y + v.Y);
        }

        public static Vec2 operator +(Vec2 u, double num)
        {
            return new Vec2(u.X + num, u.Y + num);
        }

        public static Vec2 operator +(double num, Vec2 u)
        {
            return new Vec2(u.X + num, u.Y + num);
        }

        public static Vec2 operator -(Vec2 u, Vec2 v)
        {
            return new Vec2(u.X - v.X, u.Y - v.Y);
        }

        public static Vec2 operator -(Vec2 u, double num)
        {
            return new Vec2(u.X - num, u.Y - num);
        }

        public static Vec2 operator -(double num, Vec2 u)
        {
            return new Vec2(num - u.X, num - u.Y);
        }

        public static Vec2 operator *(Vec2 u, Vec2 v)
        {
            return new Vec2(u.X * v.X, u.Y * v.Y);
        }

        public static Vec2 operator *(Vec2 u, double num)
        {
            return new Vec2(u.X * num, u.Y * num);
        }

        public static Vec2 operator *(double num, Vec2 u)
        {
            return new Vec2(num * u.X, num * u.Y);
        }

        public static Vec2 operator /(Vec2 u, Vec2 v)
        {
            return new Vec2(u.X / v.X, u.Y / v.Y);
        }

        public static Vec2 operator /(Vec2 u, double num)
        {
            return new Vec2(u.X / num, u.Y / num);
        }

        public static Vec2 operator /(double num, Vec2 u)
        {
            return new Vec2(num / u.X, num / u.Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public object Clone()
        {
            return new Vec2(this.X, this.Y);
        }

        public double Dot(Vec2 u)
        {
            return this.X * u.X + this.Y * u.Y;
        }

        public Vec2 Normalize()
        {
            return this / this.Length;
        }

        public Vec2 Mix(Vec2 v, double ratio)
        {
            return this * ratio + v * (1 - ratio);
        }

        public Vec2 Reflect(Vec2 normal)
        {
            return this - 2 * normal.Dot(this) * normal;
        }

        public Vec2 Refract(Vec2 normal, double eta)
        {
            Vec2 refracted = new Vec2();

            double ndoti = normal.Dot(this);
            double k = 1 - eta * eta * (1 - ndoti * ndoti);
            if (k >= 0)
            {
                refracted = eta * this - normal * (eta * ndoti + Math.Sqrt(k));
            }
            return refracted;
        }

        public Vec2 Rotate(double angle)
        {
            angle = angle / 180 * Math.PI;
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);

            return new Vec2(X * c - Y * s, X * s + Y * c);
        }

        public double Max()
        {
            return Math.Max(X, Y);
        }
    }
}

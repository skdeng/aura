using System;

namespace Aura.VecMath
{
    class Vec3 : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }
        public double LengthSq { get { return X * X + Y * Y + Z * Z; } }

        public double this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new ArgumentOutOfRangeException("Vec3 index must be from 0 to 2");
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Vec3 index must be from 0 to 2");
                }
            }
        }

        public Vec3()
        {
            X = Y = Z = 0;
        }

        public Vec3(double value)
        {
            X = Y = Z = value;
        }

        public Vec3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vec3(string s)
        {
            var splitted = s.Split(' ');
            X = double.Parse(splitted[0]);
            Y = double.Parse(splitted[1]);
            Z = double.Parse(splitted[2]);
        }

        public static Vec3 operator -(Vec3 u)
        {
            return new Vec3(-u.X, -u.Y, -u.Z);
        }

        public static Vec3 operator +(Vec3 u, Vec3 v)
        {
            return new Vec3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        }

        public static Vec3 operator +(Vec3 u, double num)
        {
            return u + new Vec3(num);
        }

        public static Vec3 operator +(double num, Vec3 u)
        {
            return new Vec3(num) + u;
        }

        public static Vec3 operator -(Vec3 u, Vec3 v)
        {
            return new Vec3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        }

        public static Vec3 operator -(Vec3 u, double num)
        {
            return u - new Vec3(num);
        }

        public static Vec3 operator -(double num, Vec3 u)
        {
            return new Vec3(num) - u;
        }

        public static Vec3 operator *(Vec3 u, Vec3 v)
        {
            return new Vec3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);
        }

        public static Vec3 operator *(Vec3 u, double num)
        {
            return u * new Vec3(num);
        }

        public static Vec3 operator *(double num, Vec3 u)
        {
            return new Vec3(num) * u;
        }

        public static Vec3 operator /(Vec3 u, Vec3 v)
        {
            return new Vec3(u.X / v.Z, u.Y / v.Y, u.Z / v.Z);
        }

        public static Vec3 operator /(Vec3 u, double num)
        {
            return u / new Vec3(num);
        }

        public static Vec3 operator /(double num, Vec3 u)
        {
            return new Vec3(num) / u;
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public object Clone()
        {
            return new Vec3(this.X, this.Y, this.Z);
        }

        public double Dot(Vec3 u)
        {
            return this.X * u.X + this.Y * u.Y + this.Z * u.Z;
        }

        public Vec3 Normalize()
        {
            return this / this.Length;
        }

        public Vec3 Mix(Vec3 u, double ratio)
        {
            return this * ratio + u * (1 - ratio);
        }

        public Vec3 Mix(Vec3 u, Vec3 ratio)
        {
            return this * ratio + u * (1 - ratio);
        }

        public Vec3 Cross(Vec3 u)
        {
            return new Vec3(this.Y * u.Z - this.Z * u.Y, this.Z * u.X - this.X * u.Z, this.X * u.Y - this.Y * u.X);
        }

        public Vec3 Reflect(Vec3 normal)
        {
            return this - 2 * normal.Dot(this) * normal;
        }

        public Vec3 Refract(Vec3 normal, double eta)
        {
            Vec3 refracted = new Vec3();
            double ndoti = normal.Dot(this);
            double k = 1 - eta * eta * (1 - ndoti * ndoti);
            if (k >= 0)
            {
                refracted = eta * this - normal * (eta * ndoti + Math.Sqrt(k));
            }
            return refracted;
        }

        public Vec3 Clamp(double min, double max)
        {
            return new Vec3(
                X = (X < min) ? min : ((X > max) ? max : X),
                Y = (Y < min) ? min : ((Y > max) ? max : Y),
                Z = (Z < min) ? min : ((Z > max) ? max : Z)
            );
        }

        public double Sum()
        {
            return X + Y + Z;
        }

        public Vec3 Exp()
        {
            return new Vec3(Math.Exp(X), Math.Exp(Y), Math.Exp(Z));
        }

        public Vec3 Pow(double val)
        {
            return new Vec3(Math.Pow(X, val), Math.Pow(Y, val), Math.Pow(Z, val));
        }

        public Vec3 Max(double max)
        {
            return new Vec3(Math.Max(X, max), Math.Max(Y, max), Math.Max(Z, max));
        }

        public double Max()
        {
            return Math.Max(Math.Max(X, Y), Z);
        }

        public Vec3 Min(double min)
        {
            return new Vec3(Math.Min(X, min), Math.Min(Y, min), Math.Min(Z, min));
        }

        public double Min()
        {
            return Math.Min(Math.Min(X, Y), Z);
        }

        static public Vec3 RandomHemisphereVector(Vec3 normal, Random rng)
        {
            var x = rng.NextDouble() * 2 - 1;
            var y = rng.NextDouble() * 2 - 1;
            var z = rng.NextDouble() * 2 - 1;

            var randomVector = new Vec3(x, y, z).Normalize();
            if (randomVector.Dot(normal) <= 0)
            {
                return -randomVector;
            }
            else
            {
                return randomVector;
            }
        }
    }
}

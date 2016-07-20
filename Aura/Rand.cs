using System;
using System.Numerics;

namespace Aura
{
    static class Rand
    {
        static Random Rng { get; set; }

        static Rand()
        {
            Rng = new Random();
        }

        public static double Normal()
        {
            double u1 = Rng.NextDouble();
            double u2 = Rng.NextDouble();

            return Math.Sqrt(-2.0 * Math.Log(u1) * Math.Sin(2.0 * Math.PI * u2));
        }

        public static double Normal(double mean, double stdDeviation)
        {
            return mean + stdDeviation * Normal();
        }

        public static Vector3 RandomHemisphereVector_Uniform(Vector3 normal)
        {
            var x = (float)Rng.NextDouble() * 2 - 1;
            var y = (float)Rng.NextDouble() * 2 - 1;
            var z = (float)Rng.NextDouble() * 2 - 1;

            var randomVector = Vector3.Normalize(new Vector3(x, y, z));
            if (Vector3.Dot(randomVector, normal) <= 0)
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

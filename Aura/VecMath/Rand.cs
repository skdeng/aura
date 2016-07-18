using System;

namespace Aura.VecMath
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
    }
}

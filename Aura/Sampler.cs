using Aura.VecMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura
{
    class Sampler
    {
        private int ImageWidth { get; set; }

        private int ImageHeight { get; set; }

        private int Supersample { get; set; }

        private int CurrentX { get; set; } = 0;

        private int CurrentY { get; set; } = 0;

        private int SupersampleCount { get; set; } = 0;

        private Random RNG { get; set; }
        
        public Sampler(Scene sceneDescription)
        {
            ImageWidth = sceneDescription.ImageWidth;
            ImageHeight = sceneDescription.ImageHeight;
            Supersample = sceneDescription.Supersample;
            RNG = new Random();
        }

        public Sample GetSample()
        {
            if (CurrentX >= ImageWidth || CurrentY >= ImageHeight)
            {
                CurrentX = CurrentY = SupersampleCount = 0;
                return null;
            }

            var sample = new Sample()
            {
                X = CurrentX,
                Y = CurrentY,
                OffsetX = RNG.NextDouble() * 2 - 1,
                OffsetY = RNG.NextDouble() * 2 - 1
            };

            if (++SupersampleCount < Supersample)
            {
                return sample;
            }
            SupersampleCount = 0;

            if (++CurrentX >= ImageWidth)
            {
                CurrentX = 0;
                CurrentY++;
            }

            return sample;
        }
    }

    class Sample : Vec2
    {
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }
}

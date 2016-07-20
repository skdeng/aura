using System;

namespace Aura
{
    class Sampler
    {
        private int ImageWidth { get; set; }

        private int ImageHeight { get; set; }

        private int CurrentX { get; set; } = 0;

        private int CurrentY { get; set; } = 0;

        private int SupersampleCount { get; set; } = 0;

        private Random RNG { get; set; }
        
        public Sampler(Scene sceneDescription)
        {
            ImageWidth = sceneDescription.ImageWidth;
            ImageHeight = sceneDescription.ImageHeight;
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
                OffsetX = (float)RNG.NextDouble() * 2 - 1,
                OffsetY = (float)RNG.NextDouble() * 2 - 1
            };

            if (++CurrentX >= ImageWidth)
            {
                CurrentX = 0;
                CurrentY++;
            }

            return sample;
        }

        public Sample[] GetAllSamples()
        {
            var sampleList = new Sample[ImageWidth * ImageHeight];
            Sample sample = GetSample();
            for (int i = 0; sample != null; i++)
            {
                sampleList[i] = sample;
                sample = GetSample();
            }
            return sampleList;
        }
    }

    class Sample
    {
        public int X { get; set; }
        public int Y { get; set; }

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
    }
}

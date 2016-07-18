using Aura.VecMath;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Aura
{
    class Image
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Supersample { get; private set; }

        private Vec3 BackgroundColor { get; set; }

        private Vec3[] BackBuffer { get; set; }

        private double SampleCount { get; set; } = 1;

        public WriteableBitmap SourceBitmap { get; set; }

        public Image(Scene sceneDescription)
        {
            Width = sceneDescription.ImageWidth;
            Height = sceneDescription.ImageHeight;
            Supersample = sceneDescription.Supersample;
            BackgroundColor = sceneDescription.BackgroundColor;

            BackBuffer = new Vec3[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                BackBuffer[i] = new Vec3(0);
            }
            SourceBitmap = new WriteableBitmap(Width, Height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
        }

        public void Commit(Sample sample, Vec3 color)
        {
            if (color.Max() > 1)
            {
                color = color / color.Max();
            }
            color = color / Supersample / SampleCount;

            var existingColor = BackBuffer[(int)sample.X + (int)sample.Y * Width] * SampleCount;

            BackBuffer[(int)sample.X + (int)sample.Y * Width] = existingColor + color;
        }

        public void Refresh()
        {
            IntPtr bufferPtr = new IntPtr();
            Application.Current.Dispatcher.Invoke(() =>
            {
                SourceBitmap.Lock();
                bufferPtr = SourceBitmap.BackBuffer;
            });
            unsafe
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        var currentPtr = bufferPtr + (x + y * Width) * 4;
                        *((int*)currentPtr) = ColorToRGBA(BackBuffer[x + y * Width]);
                    }
                }
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                SourceBitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                SourceBitmap.Unlock();
            });
            SampleCount += 1;
        }

        public int ColorToRGBA(Vec3 color)
        {
            int R = (int)(color.X * 255) << 16;
            int G = (int)(color.Y * 255) << 8;
            int B = (int)(color.Z * 255);
            int A = 255 << 24;
            return R | G | B | A;
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Commit(new Sample() { X = x, Y = y }, BackgroundColor);
                }
            }
        }
    }
}

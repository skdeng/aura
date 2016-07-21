using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Aura
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image ImageData { get; set; }

        private Scene SceneDescription { get; set; }

        private Camera MainCamera { get; set; }

        private Pathtracer Tracer { get; set; }

        private Sampler PixelSampler { get; set; }

        private Thread RenderThread { get; set; }

        private ManualResetEvent[] ThreadedRenderSignal { get; set; }

        private Sample[] SampleList { get; set; }

        public static Stopwatch stopwatch { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            MainImage.Source = ImageData.SourceBitmap;

            RenderThread = new Thread(new ThreadStart(Render));
            RenderThread.Start();
        }

        private void Initialize()
        {
            try
            {
                SceneDescription = new Scene();
                SceneDescription.LoadScene("../../../Scene/cornell.scene");
                PixelSampler = new Sampler(SceneDescription);
                MainCamera = new Camera(SceneDescription);
                Tracer = new Pathtracer(SceneDescription);
                ImageData = new Image(SceneDescription);

                MainCanvas.Width = SceneDescription.ImageWidth;
                MainCanvas.Height = SceneDescription.ImageHeight;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Failed to initialize the scene", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        private void Render()
        {
            int frame = 0;
            stopwatch = Stopwatch.StartNew();
            Sample sample;
            while (true)
            {
                Debug.WriteLine($"Rendering frame {frame++}");
                while ((sample = PixelSampler.GetSample()) != null)
                {
                    ImageData.Commit(sample, Tracer.Trace(MainCamera.GetRay(sample)));
                }
                ImageData.Refresh();
            }
        }

        private void Render_MT()
        {
            int frame = 0;
            stopwatch = Stopwatch.StartNew();

            ThreadedRenderSignal = new ManualResetEvent[4];

            SampleList = PixelSampler.GetAllSamples();
            var quarter = SampleList.Length / 4;
            while (true)
            {
                Debug.WriteLine($"Rendering frame {frame++}");

                for (int i = 0; i < SampleList.Length; i += quarter)
                {
                    ThreadedRenderSignal[i / quarter] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RenderSample), new ThreadArg() { Start = i, End = i + quarter - 1, Index = i / quarter });
                }
                WaitHandle.WaitAll(ThreadedRenderSignal);
                ImageData.Refresh();
            }
        }

        private void RenderSample(object s)
        {
            var arg = s as ThreadArg;
            Debug.WriteLine($"Worker {arg.Index} start {arg.Start} {arg.End}");
            for (int i = arg.Start; i < arg.End; i++)
            {
                ImageData.Commit(SampleList[i], Tracer.Trace(MainCamera.GetRay(SampleList[i])));
            }
            Debug.WriteLine($"Worker {arg.Index} end");
            ThreadedRenderSignal[arg.Index].Set();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RenderThread.Abort();
            stopwatch.Stop();
            ImageData.Save("image.bmp", "log.txt");
        }

        private class ThreadArg
        {
            public int Start, End;
            public int Index;
        }

        private void MainCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine(Mouse.GetPosition(MainImage));
        }
    }
}
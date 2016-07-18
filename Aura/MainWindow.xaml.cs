using Aura.VecMath;
using System.Diagnostics;
using System.Threading;
using System.Windows;

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
            SceneDescription = new Scene();
            SceneDescription.LoadScene("cornell.scene");
            PixelSampler = new Sampler(SceneDescription);
            MainCamera = new Camera(SceneDescription);
            Tracer = new Pathtracer(SceneDescription);
            ImageData = new Image(SceneDescription);

            MainCanvas.Width = SceneDescription.ImageWidth;
            MainCanvas.Height = SceneDescription.ImageHeight;
        }

        private void Render()
        {
            while (true)
            {
                Debug.Print("render start");
                Sample sample;
                Ray ray;
                Vec3 color;
                while ((sample = PixelSampler.GetSample()) != null)
                {
                    ray = MainCamera.GetRay(sample);
                    color = Tracer.Trace(ray);
                    ImageData.Commit(sample, color);
                }
                ImageData.Refresh();
                Debug.Print($"render end");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RenderThread.Abort();
        }
    }
}
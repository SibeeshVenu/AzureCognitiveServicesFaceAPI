using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace AzureCognitiveServiceFaceAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IFaceServiceClient _faceServiceClient = new FaceServiceClient("", "");
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            //Uploading the image

            var openFrame = new Microsoft.Win32.OpenFileDialog { Filter = "JPEG Image(*.jpg)|*.jpg" };
            var result = openFrame.ShowDialog(this);
            if (!(bool)result)
                return;
            var filePath = openFrame.FileName;
            var fileUri = new Uri(filePath);
            var bitMapSource = new BitmapImage();
            bitMapSource.BeginInit();
            bitMapSource.CacheOption = BitmapCacheOption.None;
            bitMapSource.UriSource = fileUri;
            bitMapSource.EndInit();
            FaceImage.Source = bitMapSource;

            // Detecting the faces count

            Title = "Detecting....";
            FaceRectangle[] facesFound = await DetectTheFaces(filePath);
            Title = $"Found {facesFound.Length} faces";

            // Draw rectangles

            if (facesFound.Length <= 0) return;
            var drwVisual = new DrawingVisual();
            var drwContext = drwVisual.RenderOpen();
            drwContext.DrawImage(bitMapSource, new Rect(0, 0, bitMapSource.Width, bitMapSource.Height));
            var dpi = bitMapSource.DpiX;
            var resizeFactor = 96 / dpi;
            foreach (var faceRect in facesFound)
            {
                drwContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Blue, 6),
                    new Rect(faceRect.Left * resizeFactor, faceRect.Top * resizeFactor, faceRect.Width * resizeFactor,
                        faceRect.Height * resizeFactor));
            }
            drwContext.Close();
            var renderToImageCtrl = new RenderTargetBitmap((int)(bitMapSource.PixelWidth * resizeFactor), (int)(bitMapSource.PixelHeight * resizeFactor), 96, 96, PixelFormats.Pbgra32);
            renderToImageCtrl.Render(drwVisual);
            FaceImage.Source = renderToImageCtrl;
        }

        /// <summary>
        /// Return the face rectangle counts
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<FaceRectangle[]> DetectTheFaces(string filePath)
        {
            try
            {
                using (var imgStream = File.OpenRead(filePath))
                {
                    var faces = await _faceServiceClient.DetectAsync(imgStream);
                    var faceRectangles = faces.Select(face => face.FaceRectangle);
                    return faceRectangles.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}

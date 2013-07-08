using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Identification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
                          {
                              FileName = string.Empty,
                              Filter = "Image Formats|*.bmp;*.jpg;*.jpeg;*.tif|All Files|*.*",
                              FilterIndex = 1
                          };

            if (dlg.ShowDialog().Value)
            {
                string filename = dlg.FileName;

                var bmp = new Bitmap(filename);

                SaveImage(dlg.FileName.Remove(dlg.FileName.LastIndexOf('.')), bmp);
                MessageBox.Show("Done!");
            }
        }

        private void SaveImage(string fileName, Bitmap bmp)
        {
            var image = BitmapSource.Create(
                bmp.Width,
                bmp.Height,
                bmp.BMP.DpiX,
                bmp.BMP.DpiY,
                bmp.Format,
                bmp.Palette,
                bmp.PixelsGrayScale,
                bmp.Stride);

            Save(fileName + "GS.jpg", image);

            image = BitmapSource.Create(
                bmp.Width,
                bmp.Height,
                bmp.BMP.DpiX,
                bmp.BMP.DpiY,
                bmp.Format,
                bmp.Palette,
                bmp.PixelsBlackAndWhite,
                bmp.Stride);

            Save(fileName + "BW.jpg", image);
        }

        private void Save(string fileName, BitmapSource bitmapSource)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var encoder = new JpegBitmapEncoder
                {
                    FlipHorizontal = false,
                    FlipVertical = false,
                    QualityLevel = 100,
                };

                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
            }
        }
    }
}

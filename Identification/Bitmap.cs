using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Identification
{
    public enum BlackAndWhite
    {
        Black = 0x00,
        White = 0xFF
    }

    public class Bitmap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public double DpiX { get; private set; }
        public double DpiY { get; private set; }
        public PixelFormat Format { get; private set; }
        public BitmapPalette Palette { get; private set; }
        public byte[] Pixels { get; private set; }
        public byte[] PixelsGrayScale { get; private set; }
        public byte[] PixelsBlackAndWhite { get; private set; }
        public BitmapImage BMP { get; private set; }
        
        public Bitmap(string imagePath)
        {
            var uri = new Uri(imagePath);
            BMP = new BitmapImage(uri);

            Initialize(BMP);
        }

        private void Initialize(BitmapSource bitmapSource)
        {
            Width = bitmapSource.PixelWidth;
            Height = bitmapSource.PixelHeight;
            Format = bitmapSource.Format;
            Palette = bitmapSource.Palette;
            DpiX = bitmapSource.DpiX;
            DpiY = bitmapSource.DpiY;
            Stride = (Width * bitmapSource.Format.BitsPerPixel + 7) / 8;
            Pixels = new byte[Height * Stride];
            bitmapSource.CopyPixels(Pixels, Stride, 0);

            var edge = GetEdgeValue(Pixels);

            PixelsGrayScale = new byte[Pixels.Length];
            PixelsBlackAndWhite = new byte[Pixels.Length];

            for (var pixelIndex = 0; pixelIndex < Pixels.Length; pixelIndex++ /*4*/)
            {
                //var color = new Color
                //                {
                //                    A = Pixels[pixelIndex + 3],
                //                    R = Pixels[pixelIndex + 2],
                //                    G = Pixels[pixelIndex + 1],
                //                    B = Pixels[pixelIndex]
                //                };

                var colorGrayScale = GrayScaleConverter8(Pixels[pixelIndex]/*color*/);
                PixelsGrayScale[pixelIndex] = colorGrayScale;
                //PixelsGrayScale[pixelIndex + 3] = colorGrayScale.A;
                //PixelsGrayScale[pixelIndex + 2] = colorGrayScale.R;
                //PixelsGrayScale[pixelIndex + 1] = colorGrayScale.G;
                //PixelsGrayScale[pixelIndex] = colorGrayScale.B;

                var colorBlackAndWhite = BlackAndWightConverter8(Pixels[pixelIndex]/*color*/, edge);
                PixelsBlackAndWhite[pixelIndex] = colorBlackAndWhite;
                //PixelsBlackAndWhite[pixelIndex + 3] = colorBlackAndWhite.A;
                //PixelsBlackAndWhite[pixelIndex + 2] = colorBlackAndWhite.R;
                //PixelsBlackAndWhite[pixelIndex + 1] = colorBlackAndWhite.G;
                //PixelsBlackAndWhite[pixelIndex] = colorBlackAndWhite.B;
            }
        }

        private byte GetEdgeValue(byte[] pixels)
        {
            var pixelValue = new Dictionary<int, int>();

            for (var pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex += 4)
            {
                var pixelColor = pixels[pixelIndex] + pixels[pixelIndex + 1] + pixels[pixelIndex + 2];
                if (pixelValue.ContainsKey(pixelColor))
                {
                    pixelValue[pixelColor]++;
                }
                else
                {
                    pixelValue.Add(pixelColor, 1);
                }
            }

            return 125; //(byte)(pixelValue.Sum(p => p.Key) / pixelValue.Count);
        }

        private byte GrayScaleConverter8(byte color)
        {
            var a = (byte) (color & 0xC0);
            var r = (byte)(color & 0x30);
            var g = (byte)(color & 0x0C);
            var b = (byte)(color & 0x03);

            var pixel = r;

            //var temp = (byte) ((pixel)/3);
            return r;
        }

        private byte BlackAndWightConverter8(byte color, byte edge)
        {
            var temp = (byte) ((color)/3) > edge
                           ? (byte) BlackAndWhite.White
                           : (byte) BlackAndWhite.Black;
            return temp;
        }


        private Color GrayScaleConverter(Color color)
        {
            var temp = (byte)((color.R + color.G + color.B)/3);
            return new Color {A = color.A, R = temp, G = temp, B = temp};
        }

        private Color BlackAndWightConverter(Color color, byte edge)
        {
            var temp = (byte) ((color.R + color.G + color.B)/3) > edge
                           ? (byte) BlackAndWhite.White
                           : (byte) BlackAndWhite.Black;
            return new Color {A = color.A, R = temp, G = temp, B = temp};
        }
    }
}

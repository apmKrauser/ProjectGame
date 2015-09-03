using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleGraphicsLib
{

    public static class BitmapConversion
    {

        public static Bitmap ToWinFormsBitmap(this BitmapSource bitmapsource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(stream);

                using (var tempBitmap = new Bitmap(stream))
                {
                    // According to MSDN, one "must keep the stream open for the lifetime of the Bitmap."
                    // So we return a copy of the new bitmap, allowing us to dispose both the bitmap and the stream.
                    return new Bitmap(tempBitmap);
                }
            }
        }

        public static BitmapSource ToWpfBitmap(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public static BitmapImage ToMediaBitmap(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public static BitmapImage LoadBitmapAndDispose(string path)
        {
            var bytes = File.ReadAllBytes(path);
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public static BitmapImage StringToBitmap(string str)
        {
            var text = new FormattedText(str,
                  CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,
                  new Typeface("Verdana"),
                  30, System.Windows.Media.Brushes.Green);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawRectangle(System.Windows.Media.Brushes.Transparent, null, new Rect(0, 0, 200, 100));
            drawingContext.DrawText(text, new System.Windows.Point(0, 0));
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)text.Width, (int)text.Height,100,100, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp.ToWinFormsBitmap().ToMediaBitmap();
        }

        // Geht nur in neuer Klasse die von Bitmap erbt
        //public static explicit operator BitmapImage(Bitmap bitmap)
        //{
        //    return bitmap.ToMediaBitmap();
        //}

    }
}

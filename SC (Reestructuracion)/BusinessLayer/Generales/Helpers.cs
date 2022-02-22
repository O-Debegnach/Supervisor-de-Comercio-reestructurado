using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BusinessLayer.Generales
{
    public static class Helpers
    {
        /// <summary>
        /// The controls need actual size, If they are not render an "UpdateLayout()" might be needed.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Container"></param>
        public static BitmapImage RenderToBitmap(FrameworkElement control, Size size)
        {
            if (control.ActualWidth == 0)
            {
                control.Measure(size);
                control.Arrange(new Rect(size));
                control.UpdateLayout();
            }
            // Here is where I get fired if the control was not actually rendered in the screen

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(control);

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var stream = new System.IO.MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static BitmapSource RenderToBitmap1(UIElement element, Size size)
        {
            element.Measure(size);
            element.Arrange(new Rect(size));
            element.UpdateLayout();
            var bitmap = new RenderTargetBitmap(
                (int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);

            bitmap.Render(element);
            return bitmap;
        }
    }
}

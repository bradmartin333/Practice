using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageFun
{
    internal class ImageHelper
    {
        public ActiveImage[] LoadImages(string path)
        {
            string[] images = Directory.GetFiles(path).Where(x => x.EndsWith(".jpg")).ToArray();
            List<ActiveImage> activeImages = new List<ActiveImage>();
            foreach (string image in images)
            {
                ActiveImage activeImage = new ActiveImage(image);
                if (activeImage.Format == PixelFormat.Format24bppRgb)
                    activeImages.Add(activeImage);
                
            }
            return activeImages.ToArray();
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        public static Bitmap ToBitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            Bitmap bitmap = new Bitmap(outStream);
            return new Bitmap(bitmap);
        }
    }
}

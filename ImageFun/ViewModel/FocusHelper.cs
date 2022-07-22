using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageFun
{
    internal class FocusHelper
    {
        public (double, BitmapImage)[] ScoreImages(string path)
        {
            string[] images = Directory.GetFiles(path).Where(x => x.EndsWith(".jpg")).ToArray();
            List<(double, BitmapImage)> scores = new List<(double, BitmapImage)>();
            foreach (string image in images)
            {
                ActiveImage activeImage = new ActiveImage(image);
                if (activeImage.Format != PixelFormat.Format24bppRgb) continue;
                Focus focus = new Focus(activeImage.Bitmap);
                double score = focus.ScoreImageGrid();
                focus.HighlightTiles();
                scores.Add((score, ToBitmapImage(focus.ScaledImage)));
            }
            return scores.OrderBy(x => x.Item1).Take(25).ToArray();
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
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
    }
}

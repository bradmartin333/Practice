using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace ImageFun
{
    internal class ActiveImage
    {
        public Bitmap Bitmap { get; set; }
        public BitmapImage BitmapImage { get => ImageHelper.ToBitmapImage(Bitmap); }
        public PixelFormat Format { get; set; }

        public ActiveImage(string path)
        {
            Bitmap = new Bitmap(path);
            Format = Bitmap.PixelFormat;
        }
    }
}

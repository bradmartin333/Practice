using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageFun
{
    internal class ActiveImage
    {
        public Bitmap Bitmap { get; set; }
        public string Name { get; set; }
        public PixelFormat Format { get; set; }

        public ActiveImage(string path)
        {
            Bitmap = new Bitmap(path);
            Format = Bitmap.PixelFormat;
            Name = path.Split('\\').Last().Replace(".jpg", "");
        }
    }
}

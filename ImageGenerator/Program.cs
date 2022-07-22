using System;
using System.IO;
using System.Drawing;

namespace ImageGenerator
{
    internal class Program
    {
        static readonly Random RND = new Random();
        static readonly string DIR = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\Generated\";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(DIR);
            MakeSimpleGrid();
        }

        static void MakeSimpleGrid()
        {
            for (int k = 0; k < 10; k++)
            {
                Bitmap bitmap = new Bitmap(1000, 1000);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    for (int i = 50; i < 900; i += 200)
                    {
                        for (int j = 50; j < 900; j += 200)
                        {
                            if (RND.Next(2) == 1) g.FillRectangle(
                                new SolidBrush(Color.FromArgb((int)(RND.NextDouble() * 255), Color.Black)),
                                new Rectangle(i, j, 100, 100));
                        }
                    }
                }
                bitmap.Save($@"{DIR}{k}.png");
            }
        }
    }
}

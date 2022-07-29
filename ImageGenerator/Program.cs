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
            for (int ax = 0; ax < 3; ax++)
            {
                for (int ay = 0; ay < 4; ay++)
                {
                    Console.WriteLine($"ENTER ({ax}, {ay})");
                    for (int bx = 0; bx < 5; bx++)
                    {
                        for (int by = 0; by < 6; by++)
                        {
                            Point jumble = new Point(ax * 5 + bx, ay * 3);
                            Bitmap bitmap = new Bitmap(1000, 1000);
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                g.Clear(Color.White);
                                for (int cx = 50; cx < 900; cx += 200)
                                {
                                    for (int cy = 50; cy < 900; cy += 200)
                                    {
                                        if (RND.Next(2) == 1) g.FillRectangle(
                                            new SolidBrush(Color.FromArgb((int)(RND.NextDouble() * 255), Color.Black)),
                                            new Rectangle(cx + jumble.X, cy + jumble.Y, 100, 100));
                                        g.FillRectangle(Brushes.White,
                                            new Rectangle(cx + jumble.X + 20, cy + jumble.Y, 10, 100));
                                        g.FillRectangle(Brushes.White,
                                            new Rectangle(cx + jumble.X, cy + jumble.Y + 20, 100, 10));
                                    }
                                }
                            }
                            bitmap.Save($@"{DIR}{ax + 1}_{ay + 1}_{bx + 1}_{by + 1}_{1}_{1}.png");
                        }
                    }
                }
            }   
        }
    }
}

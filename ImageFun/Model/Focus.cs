using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageFun
{
    internal class Focus
    {
        public Bitmap ScaledImage;
        private int ScanSize, Width, Height;
        private const double ImageScaling = 0.2;
        private const int GridSize = 15; // N x N grid
        private const double DataPercentage = 0.1; // Highest 10% of available data from training grid
        private int[] Tiles;

        public Focus(Bitmap bmp)
        {
            ScaledImage = new Bitmap(bmp, new Size((int)Math.Round(bmp.Width * ImageScaling), (int)Math.Round(bmp.Height * ImageScaling)));
        }

        /// Handy tool used as a method
        private void BitmapCrop(Rectangle crop, Bitmap src, ref Bitmap target)
        {
            using var g = Graphics.FromImage(target);
            g.DrawImage(src, new Rectangle(0, 0, crop.Width, crop.Height), crop, GraphicsUnit.Pixel);
        }

        // Gets grid pixels and then scores the image
        public double ScoreImageFocus(Bitmap img)
        {
            ScanSize = (int)Math.Ceiling((Math.Min(img.Height, img.Width) * (1.0 / GridSize)));
            Width = img.Width / ScanSize;
            Height = img.Height / ScanSize;

            // Get red values for every pixel in a 2D array
            double[,] PxlVals = new double[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    PxlVals[i, j] = img.GetPixel(i * ScanSize, j * ScanSize).R;

            return GetNeighborSharpness(PxlVals);
        }

        // The core of AutoFocus
        private double GetNeighborSharpness(double[,] PxlVals)
        {
            List<double> PDiff = new List<double>();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    double LocalPDiff = 0.0;
                    for (int k = i - ScanSize; k <= i + ScanSize; k++)
                        for (int l = j - ScanSize; l <= j + ScanSize; l++)
                            if (k > 0 && k < Width - 1 && l > 0 && l < Height - 1)
                            {
                                double p1 = PxlVals[i, j];
                                double p2 = PxlVals[k, l];
                                if (p1 == 0 && p2 == 0) continue;
                                LocalPDiff += Math.Abs(p1 - p2) / ((p1 + p2) / 2);
                            }
                    PDiff.Add(LocalPDiff / (ScanSize * ScanSize));
                }

            // Return average sharpness
            double score = PDiff.Sum() / PDiff.Count;
            return score;
        }

        /// <summary>
        /// Scores an image based off the tiles from a trained grid only
        /// </summary>
        /// <param name="img"></param>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public double ScoreImageGrid()
        {
            Tiles = GetTiles();
            int tileScanSize = (int)(Math.Min(ScaledImage.Height, ScaledImage.Width) * (1.0 / GridSize));
            int tileIDX = 0;
            List<double> scores = new List<double>();

            for (int i = 0; i < ScaledImage.Width; i += tileScanSize)
                for (int j = 0; j < ScaledImage.Height; j += tileScanSize)
                {
                    if (Tiles.Contains(tileIDX))
                    {
                        Bitmap tile = new Bitmap(tileScanSize, tileScanSize);
                        BitmapCrop(new Rectangle(i, j, tileScanSize, tileScanSize), ScaledImage, ref tile);
                        scores.Add(ScoreImageFocus(tile));
                    }
                    tileIDX++;

                    if (scores.Count == Tiles.Length)
                        break;
                }

            GC.Collect();
            return scores.Sum() / scores.Count();
        }

        // Returns the tiles within a grid that have entropies in the highest 2 histogram bins
        public int[] GetTiles()
        {
            int tileScanSize = (int)(Math.Min(ScaledImage.Height, ScaledImage.Width) * (1.0 / GridSize));
            Dictionary<int, double> entropyDict = new Dictionary<int, double>();
            int entropyIDX = 0;

            for (int i = 0; i < ScaledImage.Width; i += tileScanSize)
                for (int j = 0; j < ScaledImage.Height; j += tileScanSize)
                {
                    Bitmap tile = new Bitmap(tileScanSize, tileScanSize);
                    BitmapCrop(new Rectangle(i, j, tileScanSize, tileScanSize), ScaledImage, ref tile);
                    List<double> entropyList = new List<double>();
                    for (int k = 0; k < tile.Width; k++)
                        for (int l = 0; l < tile.Height; l++)
                            entropyList.Add(tile.GetPixel(k, l).ToArgb());
                    entropyDict.Add(entropyIDX, Entropy(entropyList));
                    entropyIDX++;
                }

            IEnumerable<int> sortedTiles = entropyDict.OrderBy(x => x.Value).Select(x => x.Key).Reverse();
            int[] tiles = sortedTiles.Take((int)(sortedTiles.Count() * DataPercentage)).ToArray();
            return tiles;
        }

        /// <summary>
        /// Colorize with the grid layout of the grid training image
        /// </summary>
        public void HighlightTiles()
        {
            int tileScanSize = (int)(Math.Min(ScaledImage.Height, ScaledImage.Width) * (1.0 / GridSize));
            int tileIDX = 0;
            using Graphics g = Graphics.FromImage(ScaledImage);
            for (int i = 0; i < ScaledImage.Width; i += tileScanSize)
                for (int j = 0; j < ScaledImage.Height; j += tileScanSize)
                {
                    if (Tiles.Contains(tileIDX))
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), new Rectangle(i, j, tileScanSize, tileScanSize));
                    tileIDX++;
                }
        }

        /// <summary>
        /// This is what lies behind MathNet's Entropy calculation
        /// </summary>
        /// <param name="redPixels">
        /// An image deconstructed into a list of it's Red channel pixel intensities
        /// </param>
        /// <returns>
        /// Calculated Entropy
        /// </returns>
        public static double Entropy(List<double> redPixels)
        {
            Dictionary<double, double> dictionary = new Dictionary<double, double>();
            int num = 0;
            foreach (double num2 in redPixels)
            {
                if (double.IsNaN(num2))
                    return double.NaN;
                if (dictionary.TryGetValue(num2, out double num3))
                    num3 = (dictionary[num2] = num3 + 1.0);
                else
                    dictionary.Add(num2, 1.0);
                num++;
            }
            double num4 = 0.0;
            foreach (KeyValuePair<double, double> keyValuePair in dictionary)
            {
                double num5 = keyValuePair.Value / (double)num;
                num4 += num5 * Math.Log(num5, 2.0);
            }
            return -num4;
        }
    }
}

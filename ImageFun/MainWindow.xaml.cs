using System;
using System.Windows;
using System.Windows.Controls;

namespace ImageFun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadImages();
        }

        private void LoadImages()
        {
            ImageHelper imageHelper = new ImageHelper();
            ActiveImage[] activeImages = imageHelper.LoadImages($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\Dev2\");
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    Image image = new Image()
                    {
                        Margin = new Thickness(5),
                        Stretch = System.Windows.Media.Stretch.Fill,
                        Source = activeImages[(i * 5) + j].BitmapImage,
                        Tag = "0",
                    };
                    image.MouseUp += Image_MouseUp;
                    mainGrid.Children.Add(image);
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);
                }
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image.Tag.ToString() == "0")
            {
                Focus focus = new Focus(ImageHelper.ToBitmap((System.Windows.Media.Imaging.BitmapImage)image.Source));
                focus.ScoreImageGrid();
                focus.HighlightTiles();
                image.Source = ImageHelper.ToBitmapImage(focus.ScaledImage);
            }
        }
    }
}

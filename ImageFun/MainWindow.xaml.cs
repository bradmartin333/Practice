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
            ActiveImage[] activeImages = ImageHelper.LoadImages($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\Dev2\");
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Image image = new Image()
                    {
                        Margin = new Thickness(5),
                        Stretch = System.Windows.Media.Stretch.Uniform,
                        Source = activeImages[(i * 3) + j].BitmapImage,
                        Tag = "0",
                    };
                    image.MouseUp += Image_MouseUp;
                    mainGrid.Children.Add(image);
                    Grid.SetRow(image, (i * 2) + 1);
                    Grid.SetColumn(image, j);
                }
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image.Tag.ToString() == "0")
            {
                image.Tag = "1";

                Focus focus = new Focus(ImageHelper.ToBitmap((System.Windows.Media.Imaging.BitmapImage)image.Source));
                image.Source = ImageHelper.ToBitmapImage(focus.ScaledImage);

                int row = Grid.GetRow(image);
                int col = Grid.GetColumn(image);

                Label label = new Label()
                {
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Content = focus.ElapsedTime,
                };
                mainGrid.Children.Add(label);
                Grid.SetRow(label, row - 1);
                Grid.SetColumn(label, col);
            }
        }
    }
}

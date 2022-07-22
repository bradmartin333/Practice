using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            FocusHelper focusHelper = new FocusHelper();
            (double, BitmapImage)[] data = focusHelper.ScoreImages($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\Dev\");
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Image image = new Image()
                    {
                        Margin = new Thickness(5),
                        Stretch = System.Windows.Media.Stretch.Fill,
                        Source = data[(i * 5) + j].Item2,
                    };
                    mainGrid.Children.Add(image);
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);
                }
            }
        }
    }
}

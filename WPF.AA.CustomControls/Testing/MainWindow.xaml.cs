using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF.AA.CustomControls;
using WPF.AA.CustomControls.ColorSpace;

namespace Testing
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel();

            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //viewModel = new MainWindowViewModel();

            //DataContext = viewModel;

            //HSV hsv = new HSV
            //{
            //    H = 240,
            //    S = 0.2f,
            //    V = 0.0217f
            //};
            //Color hsvToColor = hsv.ToMediaColor();
            //HSV convertedHSV = hsvToColor.ToHsv();
            //Color reConvertedColor = convertedHSV.ToMediaColor();
            //Debug.WriteLine("Colors");
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF.AA.CustomControls.ColorSpace;

namespace Testing
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainWindowViewModel
            {
                Value = 81
            };

            DataContext = viewModel;

            // generate color gradient
            List<Color> colors = new List<Color>();

            for (int h = 0; h < 360; h += 60) 
            {
                colors.Add(WPF.AA.CustomControls.ColorSpace.ColorConverter.ConvertHsvToRgb(new WPF.AA.CustomControls.ColorSpace.HSV { H = h, S = 1, V = 1 }));
            }

            colors.Add(WPF.AA.CustomControls.ColorSpace.ColorConverter.ConvertHsvToRgb(new WPF.AA.CustomControls.ColorSpace.HSV { H = 0, S = 1, V = 1 }));

            GradientStopCollection gradients = new GradientStopCollection();
            
            double offset = 0;

            foreach (Color c in colors) 
            {
                gradients.Add(new GradientStop(c, offset));

                offset += 0.1666667;

                if (offset > 1) offset = 1;
            }

            codeGened.Background = new LinearGradientBrush(gradients, new Point(0, 0), new Point(1, 0));

            List<Color> colors2 = new List<Color>();

            for (int h = 0; h < 360; h += 60)
            {
                colors2.Add(WPF.AA.CustomControls.ColorSpace.ColorConverter.ConvertHslToRgb(new WPF.AA.CustomControls.ColorSpace.HSL { H = h, S = 1, L = 0.5f }));
            }

            colors2.Add(WPF.AA.CustomControls.ColorSpace.ColorConverter.ConvertHslToRgb(new WPF.AA.CustomControls.ColorSpace.HSL { H = 0, S = 1, L = 0.5f }));

            GradientStopCollection gradients2 = new GradientStopCollection();

            double offset2 = 0;

            foreach (Color c in colors2)
            {
                gradients2.Add(new GradientStop(c, offset2));

                offset2 += 0.1666667;

                if (offset2 > 1) offset2 = 1;
            }

            codeGened2.Background = new LinearGradientBrush(gradients2, new Point(0, 0), new Point(1, 0));
        }
    }
}

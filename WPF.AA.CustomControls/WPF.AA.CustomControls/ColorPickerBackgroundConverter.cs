using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF.AA.CustomControls
{
    /// <summary>Converts the color from the ColorSlider into a LinearGradientBrush for the picker.</summary>
    public class ColorPickerBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LinearGradientBrush lgb = new LinearGradientBrush(Colors.White, (Color)value, new Point(0, 0), new Point(1, 0));

            return lgb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as LinearGradientBrush).GradientStops[1].Color;
        }
    }
}

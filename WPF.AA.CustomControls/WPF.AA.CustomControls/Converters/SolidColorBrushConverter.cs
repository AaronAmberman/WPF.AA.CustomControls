using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF.AA.CustomControls.Converters
{
    /// <summary>Converts a Windows.Media.Color to a SolidColorBrush.</summary>
    public class SolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }
    }
}

using System.Windows;
using System.Windows.Controls;

namespace WPF.AA.CustomControls
{
    /// <summary>A simple RBG color picker control for WPF.</summary>
    public class ColorPicker : Control
    {
        // https://stackoverflow.com/questions/32513387/how-to-create-a-color-canvas-for-color-picker-wpf
        // https://www.codeproject.com/Articles/36802/WPF-Colour-Slider
            // https://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=36802
        // https://bootcamp.uxdesign.cc/definitive-guide-to-wpf-colors-color-spaces-color-pickers-and-creating-your-own-colors-for-mere-f480935c6e94

        #region Properties

        /// <summary>Gets or sets the corner radius for the button.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ColorPicker), new PropertyMetadata(null));

        #endregion

        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        #endregion
    }
}

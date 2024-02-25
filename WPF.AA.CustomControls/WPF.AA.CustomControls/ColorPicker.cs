using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF.AA.CustomControls
{
    /// <summary>A simple RBG color picker control for WPF.</summary>
    [TemplatePart(Name = "PART_ColorRectangle", Type = typeof(Rectangle))]
    public class ColorPicker : Control
    {
        // https://stackoverflow.com/questions/32513387/how-to-create-a-color-canvas-for-color-picker-wpf
        // https://www.codeproject.com/Articles/36802/WPF-Colour-Slider
        // https://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=36802
        // https://bootcamp.uxdesign.cc/definitive-guide-to-wpf-colors-color-spaces-color-pickers-and-creating-your-own-colors-for-mere-f480935c6e94

        #region Fields

        //private BitmapSource colorGradientImage;
        //private Rectangle colorRectangle;
        private Border colorSquare;

        #endregion

        #region Properties

        /// <summary>Gets or sets the base color (the color for the vertical color slider).</summary>
        public Color BaseColor
        {
            get { return (Color)GetValue(BaseColorProperty); }
            set { SetValue(BaseColorProperty, value); }
        }

        public static readonly DependencyProperty BaseColorProperty =
            DependencyProperty.Register("BaseColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Red));

        /// <summary>Gets or sets the corner radius for the button.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary>Gets or sets the selected color (the color for the gradient color picker).</summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Red));

        #endregion

        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            colorSquare = GetTemplateChild("PART_ColorSquare") as Border;
            colorSquare.PreviewMouseLeftButtonDown += ColorSquare_PreviewMouseLeftButtonDown;

            base.OnApplyTemplate();
        }

        private void ColorSquare_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(colorSquare);
            BitmapSource gradientImage = colorSquare.CaptureAsImage();

            // get the color from the image at the click point
            CroppedBitmap cb = new CroppedBitmap(gradientImage, new Int32Rect((int)clickPoint.X, (int)clickPoint.Y, 1, 1));
            byte[] rgb = new byte[4];

            cb.CopyPixels(rgb, 4, 0);

            // gen color from read pixel color
            Color clickedColor = Color.FromRgb(rgb[2], rgb[1], rgb[0]);

            Debug.WriteLine($"The clicked on color is: {clickedColor}");
        }

        #endregion
    }
}

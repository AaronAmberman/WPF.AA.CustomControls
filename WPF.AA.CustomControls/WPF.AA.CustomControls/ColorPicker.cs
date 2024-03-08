using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPF.AA.CustomControls.ColorSpace;

namespace WPF.AA.CustomControls
{
    /// <summary>A simple RBG color picker control for WPF.</summary>
    [TemplatePart(Name = "PART_ColorSquare", Type = typeof(Border))]
    public class ColorPicker : Control
    {
        // https://stackoverflow.com/questions/32513387/how-to-create-a-color-canvas-for-color-picker-wpf
        // https://www.codeproject.com/Articles/36802/WPF-Colour-Slider
        // https://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=36802
        // https://bootcamp.uxdesign.cc/definitive-guide-to-wpf-colors-color-spaces-color-pickers-and-creating-your-own-colors-for-mere-f480935c6e94

        #region Fields

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
            DependencyProperty.Register("BaseColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent));

        /// <summary>Gets or sets the corner radius for the button.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Gets or sets the color hex code.</summary>
        public string HexStringCode
        {
            get { return (string)GetValue(HexStringCodeProperty); }
            set { SetValue(HexStringCodeProperty, value); }
        }

        public static readonly DependencyProperty HexStringCodeProperty =
            DependencyProperty.Register("HexStringCode", typeof(string), typeof(ColorPicker), new PropertyMetadata("#00000000"));

        /// <summary>Gets or sets the previous color.</summary>
        public Color PreviousColor
        {
            get { return (Color)GetValue(PreviousColorProperty); }
            set { SetValue(PreviousColorProperty, value); }
        }

        public static readonly DependencyProperty PreviousColorProperty =
            DependencyProperty.Register("PreviousColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent));

        /// <summary>Gets or sets the selected color (the color for the gradient color picker).</summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent));

        /// <summary>Gets or sets the R slider value.</summary>
        public int SliderRValue
        {
            get { return (int)GetValue(SliderRValueProperty); }
            set { SetValue(SliderRValueProperty, value); }
        }

        public static readonly DependencyProperty SliderRValueProperty =
            DependencyProperty.Register("SliderRValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0));

        /// <summary>Gets or sets the G slider value.</summary>
        public int SliderGValue
        {
            get { return (int)GetValue(SliderGValueProperty); }
            set { SetValue(SliderGValueProperty, value); }
        }

        public static readonly DependencyProperty SliderGValueProperty =
            DependencyProperty.Register("SliderGValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0));

        /// <summary>Gets or sets the B slider value.</summary>
        public int SliderBValue
        {
            get { return (int)GetValue(SliderBValueProperty); }
            set { SetValue(SliderBValueProperty, value); }
        }

        public static readonly DependencyProperty SliderBValueProperty =
            DependencyProperty.Register("SliderBValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0));

        /// <summary>Gets or sets the A slider value.</summary>
        public int SliderAValue
        {
            get { return (int)GetValue(SliderAValueProperty); }
            set { SetValue(SliderAValueProperty, value); }
        }

        public static readonly DependencyProperty SliderAValueProperty =
            DependencyProperty.Register("SliderAValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0));

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

            // if we have a SelectedColor prior to having our template applied then we need to set the BaseColor and PreviousColor
            if (SelectedColor != Colors.Transparent)
            {
                BaseColor = SelectedColor;
                PreviousColor = SelectedColor;
            }
        }

        private void ColorSquare_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(colorSquare);            
            float xPercentage = (float)(point.X / colorSquare.ActualWidth);
            float yPercentage = (float)Math.Abs((point.Y / colorSquare.ActualHeight) - 1); // we want to invert the y so take the absolute value - 1

            HSV hsv = new HSV
            {
                H = BaseColor.ToHsv().H,
                S = xPercentage,
                V = yPercentage
            };

            SelectedColor = hsv.ToMediaColor();
        }

        #endregion
    }
}

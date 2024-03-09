using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF.AA.CustomControls.ColorSpace;

namespace WPF.AA.CustomControls
{
    /// <summary>A simple RBG color picker control for WPF.</summary>
    [TemplatePart(Name = "PART_ColorSquare", Type = typeof(Border))]
    [TemplatePart(Name = "PART_WhiteSquare", Type = typeof(Border))]
    [TemplatePart(Name = "PART_BlackSquare", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ATextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_BTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_GTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_RTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_HexTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_InnerCircle", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_OuterCircle", Type = typeof(Ellipse))]
    public class ColorPicker : Control
    {
        #region Fields

        private Border blackSquare;
        private Canvas canvas;
        private Ellipse canvasInnerCircle;
        private Ellipse canvasOuterCircle;
        private Border colorSquare;
        private TextBox hexTextBox;
        private bool isBeingUpdated;
        private TextBox textBoxA;
        private TextBox textBoxB;
        private TextBox textBoxG;
        private TextBox textBoxR;
        private Border whiteSquare;

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

        /// <summary>Gets or sets the cursor to show when the user mouses over the square color picker portion of the control.</summary>
        public Cursor ColorPickerCursor
        {
            get { return (Cursor)GetValue(ColorPickerCursorProperty); }
            set { SetValue(ColorPickerCursorProperty, value); }
        }

        public static readonly DependencyProperty ColorPickerCursorProperty =
            DependencyProperty.Register("ColorPickerCursor", typeof(Cursor), typeof(ColorPicker), new PropertyMetadata(Cursors.Pen));

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
            DependencyProperty.Register("HexStringCode", typeof(string), typeof(ColorPicker), new PropertyMetadata("#00000000", HexColorChanged));

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
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent, SelectedColorChanged));

        /// <summary>Gets or sets the R slider value.</summary>
        public int SliderRValue
        {
            get { return (int)GetValue(SliderRValueProperty); }
            set { SetValue(SliderRValueProperty, value); }
        }

        public static readonly DependencyProperty SliderRValueProperty =
            DependencyProperty.Register("SliderRValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, RValueChanged, CoerceRValue));

        /// <summary>Gets or sets the G slider value.</summary>
        public int SliderGValue
        {
            get { return (int)GetValue(SliderGValueProperty); }
            set { SetValue(SliderGValueProperty, value); }
        }

        public static readonly DependencyProperty SliderGValueProperty =
            DependencyProperty.Register("SliderGValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, GValueChanged, CoerceGValue));

        /// <summary>Gets or sets the B slider value.</summary>
        public int SliderBValue
        {
            get { return (int)GetValue(SliderBValueProperty); }
            set { SetValue(SliderBValueProperty, value); }
        }

        public static readonly DependencyProperty SliderBValueProperty =
            DependencyProperty.Register("SliderBValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, BValueChanged, CoerceBValue));

        /// <summary>Gets or sets the A slider value.</summary>
        public int SliderAValue
        {
            get { return (int)GetValue(SliderAValueProperty); }
            set { SetValue(SliderAValueProperty, value); }
        }

        public static readonly DependencyProperty SliderAValueProperty =
            DependencyProperty.Register("SliderAValue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, AValueChanged, CoerceAValue));

        #endregion

        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public ColorPicker()
        {
            Loaded += ColorPicker_Loaded;
        }

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColorCircle();
        }

        #endregion

        #region Methods

        private void ARGBTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // only allow certain keys
            if (!(e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 ||
                e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 ||
                e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 ||
                e.Key == Key.Tab || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.NumLock ||
                (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab)))
            {
                e.Handled = true;
            }
        }

        private void BlackSquare_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedColor = Colors.Black;

            UpdateColorCircle();
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

            UpdateColorCircle();
        }

        private static void ConvertHexCode(ColorPicker picker, string hexCode)
        {
            try
            {
                Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(hexCode);

                picker.isBeingUpdated = true;
                picker.SelectedColor = color;
                picker.SliderRValue = color.R;
                picker.SliderGValue = color.G;
                picker.SliderBValue = color.B;
                picker.SliderAValue = color.A;
                picker.isBeingUpdated = false;

                picker.UpdateColorCircle();
            }
            catch (FormatException fe)
            {
                Debug.WriteLine($"An exception occurred attempting to convert the hex string to a color.{Environment.NewLine}{fe}");

                // just don't do anything programmatically, we just won't set the color...because we can't
            }
        }

        private static object CoerceAValue(DependencyObject d, object baseValue)
        {
            return CoerceSliderValue(d, baseValue);
        }

        private static object CoerceBValue(DependencyObject d, object baseValue)
        {
            return CoerceSliderValue(d, baseValue);
        }

        private static object CoerceGValue(DependencyObject d, object baseValue)
        {
            return CoerceSliderValue(d, baseValue);
        }

        private static object CoerceRValue(DependencyObject d, object baseValue)
        {
            return CoerceSliderValue(d, baseValue);
        }

        private static object CoerceSliderValue(DependencyObject d, object baseValue)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return baseValue;
            if (baseValue == null) return 0;

            int val = (int)baseValue;

            if (val < 0) return 0;
            else if (val > 255) return 255;

            if (string.IsNullOrWhiteSpace(baseValue.ToString())) return 0;

            return baseValue;
        }

        private static void HexColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            /*
             * there are acceptable lengths for hex colors...
             * 
             * With #...
             *     7 - indicates a color with RGB and alpha will be set to 255
             *     9 - indicates a color with ARGB
             *     
             * Without #...
             *     6 - indicates a color with RGB and alpha will be set to 255
             *     8 - indicates a color with ARGB
             */
            if (picker.HexStringCode.StartsWith('#'))
            {
                if (picker.HexStringCode.Length == 7 || picker.HexStringCode.Length == 9)
                {
                    ConvertHexCode(picker, picker.HexStringCode);
                }
            }
            else
            {
                if (picker.HexStringCode.Length == 6 || picker.HexStringCode.Length == 8)
                {
                    ConvertHexCode(picker, $"#{picker.HexStringCode}");
                }
            }
        }

        private void HexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // only allow certain keys
            if (!(e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 ||
                e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 ||
                e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 ||
                e.Key == Key.Tab || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.NumLock || e.Key == Key.CapsLock ||
                e.Key == Key.A || e.Key == Key.B || e.Key == Key.C || e.Key == Key.D || e.Key == Key.E || e.Key == Key.F ||
                (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab) ||
                (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V) /* allow paste */))
            {
                e.Handled = true;
            }
        }

        public override void OnApplyTemplate()
        {
            colorSquare = GetTemplateChild("PART_ColorSquare") as Border;
            colorSquare.PreviewMouseLeftButtonDown += ColorSquare_PreviewMouseLeftButtonDown;

            whiteSquare = GetTemplateChild("PART_WhiteSquare") as Border;
            whiteSquare.PreviewMouseLeftButtonDown += WhiteSquare_PreviewMouseLeftButtonDown;

            blackSquare = GetTemplateChild("PART_BlackSquare") as Border;
            blackSquare.PreviewMouseLeftButtonDown += BlackSquare_PreviewMouseLeftButtonDown;

            textBoxA = GetTemplateChild("PART_ATextBox") as TextBox;
            textBoxA.KeyDown += ARGBTextBox_KeyDown;

            textBoxB = GetTemplateChild("PART_BTextBox") as TextBox;
            textBoxB.KeyDown += ARGBTextBox_KeyDown;

            textBoxG = GetTemplateChild("PART_GTextBox") as TextBox;
            textBoxG.KeyDown += ARGBTextBox_KeyDown;

            textBoxR = GetTemplateChild("PART_RTextBox") as TextBox;
            textBoxR.KeyDown += ARGBTextBox_KeyDown;

            hexTextBox = GetTemplateChild("PART_HexTextBox") as TextBox;
            hexTextBox.KeyDown += HexTextBox_KeyDown; ;

            canvas = GetTemplateChild("PART_Canvas") as Canvas;
            canvasInnerCircle = GetTemplateChild("PART_InnerCircle") as Ellipse;
            canvasOuterCircle = GetTemplateChild("PART_OuterCircle") as Ellipse;

            base.OnApplyTemplate();

            // if we have a SelectedColor prior to having our template applied then we need to set the BaseColor and PreviousColor
            if (SelectedColor != Colors.Transparent)
            {
                BaseColor = new Color
                {
                    A = 255,
                    R = SelectedColor.R,
                    G = SelectedColor.G,
                    B = SelectedColor.B
                };
                PreviousColor = SelectedColor;
            }
        }

        private static void SelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            Color newColor = (Color)e.NewValue;

            picker.isBeingUpdated = true;
            picker.SliderRValue = newColor.R;
            picker.SliderGValue = newColor.G;
            picker.SliderBValue = newColor.B;
            picker.SliderAValue = newColor.A;
            picker.HexStringCode = $"#{newColor.A:X2}{newColor.R:X2}{newColor.G:X2}{newColor.B:X2}";
            picker.isBeingUpdated = false;
        }

        private static void RValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            byte newValue = Convert.ToByte(e.NewValue);

            UpdateColorAndHexCode(picker, (byte)picker.SliderAValue, newValue, (byte)picker.SliderGValue, (byte)picker.SliderBValue);
        }

        private static void GValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            byte newValue = Convert.ToByte(e.NewValue);

            UpdateColorAndHexCode(picker, (byte)picker.SliderAValue, (byte)picker.SliderRValue, newValue, (byte)picker.SliderBValue);
        }

        private static void BValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            byte newValue = Convert.ToByte(e.NewValue);

            UpdateColorAndHexCode(picker, (byte)picker.SliderAValue, (byte)picker.SliderRValue, (byte)picker.SliderGValue, newValue);
        }

        private static void AValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            byte newValue = Convert.ToByte(e.NewValue);

            UpdateColorAndHexCode(picker, newValue, (byte)picker.SliderRValue, (byte)picker.SliderGValue, (byte)picker.SliderBValue);
        }

        private static void UpdateColorAndHexCode(ColorPicker picker, byte a, byte r, byte g, byte b)
        {
            picker.isBeingUpdated = true;
            picker.SelectedColor = new Color
            {
                A = a,
                R = r,
                G = g,
                B = b
            };
            picker.HexStringCode = $"#{a:X2}{r:X2}{g:X2}{b:X2}";
            picker.isBeingUpdated = false;

            picker.UpdateColorCircle();
        }

        private void UpdateColorCircle()
        {
            HSV hsv = SelectedColor.ToHsv();

            Debug.WriteLine($"S:{hsv.S} || V:{hsv.V}");

            //float xPercentage;

            //if (hsv.S <= 0) // 0
            //{
            //    xPercentage = 10;
            //}
            //else if (hsv.S >= 1) // 1
            //{
            //    xPercentage = (float)colorSquare.ActualWidth + 10;
            //}
            //else // between 0 and 1
            //{
            //    xPercentage = ((float)colorSquare.ActualWidth * hsv.S) + 10;
            //}

            float xPercentage = ((float)colorSquare.ActualWidth * hsv.S) + 10;

            //float yPercentage;

            //if (hsv.V <= 0) // 0
            //{
            //    yPercentage = (float)colorSquare.ActualHeight + 10;
            //}
            //else if (hsv.V >= 1) // 1
            //{
            //    yPercentage = 10;
            //}
            //else // between 0 and 1
            //{
            //    yPercentage = Math.Abs(((float)colorSquare.ActualHeight * hsv.V) - (float)colorSquare.ActualHeight) + 10;
            //}

            float yPercentage = Math.Abs(((float)colorSquare.ActualHeight * hsv.V) - (float)colorSquare.ActualHeight) + 10;

            Debug.WriteLine($"X:{xPercentage} || Y:{yPercentage}");

            Canvas.SetLeft(canvasInnerCircle, xPercentage - 5);
            Canvas.SetTop(canvasInnerCircle, yPercentage - 5);

            Canvas.SetLeft(canvasOuterCircle, xPercentage - 6);
            Canvas.SetTop(canvasOuterCircle, yPercentage - 6);
        }

        private void WhiteSquare_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedColor = Colors.White;

            UpdateColorCircle();
        }

        #endregion
    }
}

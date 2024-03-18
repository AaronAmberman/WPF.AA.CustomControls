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
    [TemplatePart(Name = "PART_InnerCircle", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_OuterCircle", Type = typeof(Ellipse))]
    public class ColorPicker : Control
    {
        #region Fields

        private Border blackSquare;
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

        /// <summary>Gets or sets the cursor to show when the user mouses over the square color picker portion of the control.</summary>
        public Cursor ColorPickerCursor
        {
            get { return (Cursor)GetValue(ColorPickerCursorProperty); }
            set { SetValue(ColorPickerCursorProperty, value); }
        }

        public static readonly DependencyProperty ColorPickerCursorProperty =
            DependencyProperty.Register("ColorPickerCursor", typeof(Cursor), typeof(ColorPicker), new PropertyMetadata(Cursors.Pen));

        /// <summary>Gets or sets the corner radius for the ColorPicker.</summary>
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

        /// <summary>Gets or sets the hue color (the color for the vertical color slider).</summary>
        /// <remarks>
        /// The developer should not need to manage this property too much or at all. When the selected color is set we strip the hue value out of 
        /// it and assign that value here so that it reflects on the vertical slider. Devs can set this externally but it is not suggested. Instead
        /// just manage the SelectedColor.
        /// </remarks>
        public Color HueColor
        {
            get { return (Color)GetValue(HueColorProperty); }
            set { SetValue(HueColorProperty, value); }
        }

        public static readonly DependencyProperty HueColorProperty =
            DependencyProperty.Register("HueColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent, HueColorChanged));

        /// <summary>Gets or sets the previous color.</summary>
        /// <remarks>
        /// This color is not managed at all. This is to be managed externally so the dev using this can decide if they want have the previous color
        /// update in real time or at some other point. Loaded is not enough because this control might not be in a dialog and loaded will only fire
        /// when the control is rendered to the screen. So rather than come up with some internal strategy for managing the PreviousColor we'll just
        /// leave it up to the dev implementing the ColorPicker.
        /// </remarks>
        public Color PreviousColor
        {
            get { return (Color)GetValue(PreviousColorProperty); }
            set { SetValue(PreviousColorProperty, value); }
        }

        public static readonly DependencyProperty PreviousColorProperty =
            DependencyProperty.Register("PreviousColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent));

        /// <summary>Gets or sets the selected color.</summary>
        /// <remarks>
        /// When this property is set the hue value is stripped out of it and then set on the HueColor. This way the HueColor will always reflect the 
        /// more core hue of the SelectedColor. Clearly, this won't work with colors not on the vertical slider; such as grays, whites and blacks.
        /// </remarks>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Transparent, SelectedColorChangedCallback));

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

        #region Events

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPicker));

        /// <summary>Occurs when the selected color changes.</summary>
        public event RoutedEventHandler SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        #endregion

        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public ColorPicker()
        {
            Loaded += ColorPicker_Loaded;
            SizeChanged += ColorPicker_SizeChanged;
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

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedColor != Colors.Transparent)
                UpdateColorCircle();
        }

        private void ColorPicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (canvasInnerCircle == null || canvasOuterCircle == null || colorSquare == null) return;

            UpdateColorCircle();
        }

        private void ColorSquare_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(colorSquare);
            float xPercentage = (float)(point.X / colorSquare.ActualWidth);
            float yPercentage = (float)Math.Abs((point.Y / colorSquare.ActualHeight) - 1); // we want to invert the y so take the absolute value - 1

            HSV hsv = new HSV
            {
                H = HueColor.ToHsv().H,
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
                HSV hsv = color.ToHsv();
                Color hueColor = new HSV { H = hsv.H, S = 1, V = 1 }.ToMediaColor();

                picker.isBeingUpdated = true;
                picker.HueColor = hueColor;
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

        private static void HueColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;

            if (picker == null) return;
            if (picker.isBeingUpdated) return; // update originated else where that will handle the result

            Color newColor = (Color)e.NewValue;

            picker.isBeingUpdated = true;

            HSV selectedColorHsv = picker.SelectedColor.ToHsv();
            HSV hsv = newColor.ToHsv();

            // update the hue of the selected color to match our hue but leave the S and V values the same
            selectedColorHsv.H = hsv.H;

            Color newSelectedColor = selectedColorHsv.ToMediaColor();

            picker.SelectedColor = newSelectedColor;
            picker.SliderRValue = newSelectedColor.R;
            picker.SliderGValue = newSelectedColor.G;
            picker.SliderBValue = newSelectedColor.B;
            picker.SliderAValue = newSelectedColor.A;
            picker.HexStringCode = $"#{newSelectedColor.A:X2}{newSelectedColor.R:X2}{newSelectedColor.G:X2}{newSelectedColor.B:X2}";

            picker.isBeingUpdated = false;
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
            hexTextBox.KeyDown += HexTextBox_KeyDown;

            canvasInnerCircle = GetTemplateChild("PART_InnerCircle") as Ellipse;
            canvasOuterCircle = GetTemplateChild("PART_OuterCircle") as Ellipse;

            base.OnApplyTemplate();
        }

        private static void SelectedColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

            // get hue correct color for color slider
            HSV hsv = picker.SelectedColor.ToHsv();
            Color hueCorrectedColor = new HSV
            {
                H = hsv.H,
                S = 1,
                V = 1
            }.ToMediaColor();

            picker.HueColor = hueCorrectedColor;
            picker.isBeingUpdated = false;
            picker.RaiseEvent(new RoutedEventArgs(SelectedColorChangedEvent));
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

            HSV hsv = picker.SelectedColor.ToHsv();

            picker.HueColor = new HSV { H = hsv.H, S = 1, V = 1 }.ToMediaColor();
            picker.HexStringCode = $"#{a:X2}{r:X2}{g:X2}{b:X2}";
            picker.isBeingUpdated = false;

            picker.UpdateColorCircle();
        }

        private void UpdateColorCircle()
        {
            HSV hsv = SelectedColor.ToHsv();

            //Debug.WriteLine($"S:{hsv.S} || V:{hsv.V}");

            float xPercentage = ((float)colorSquare.ActualWidth * hsv.S) + 10;
            float yPercentage = Math.Abs(((float)colorSquare.ActualHeight * hsv.V) - (float)colorSquare.ActualHeight) + 10;

            //Debug.WriteLine($"X:{xPercentage} || Y:{yPercentage}");

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

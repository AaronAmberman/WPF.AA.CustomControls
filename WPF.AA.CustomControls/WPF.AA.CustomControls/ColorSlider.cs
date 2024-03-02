using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.AA.CustomControls
{
    /// <summary>A color slider control.</summary>
    [TemplatePart(Name = "PART_ColorGradient", Type = typeof(Border))]
    public class ColorSlider : Slider
    {
        #region Fields

        private Border colorGradient;
        private BitmapSource colorGradientImage;
        private bool isBeingUpdated;

        #endregion

        #region Properties

        /// <summary>Gets or sets the selected color.</summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Red, SelectedColorChangedCallback));

        #endregion

        #region Events

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorSlider));

        /// <summary>Occurs when the selected color changes.</summary>
        public event RoutedEventHandler SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        /// <summary>Gets or sets the style for the thumb portion of the control.</summary>
        public Style ThumbStyle
        {
            get { return (Style)GetValue(ThumbStyleProperty); }
            set { SetValue(ThumbStyleProperty, value); }
        }

        public static readonly DependencyProperty ThumbStyleProperty =
            DependencyProperty.Register("ThumbStyle", typeof(Style), typeof(ColorSlider), new PropertyMetadata(null));

        #endregion

        #region Constructors

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        /// <summary>Initializes a new instance of the <see cref="ColorSlider"/> class.</summary>
        public ColorSlider()
        {
            Loaded += ColorSlider_Loaded;
        }

        #endregion

        #region Methods

        private void ColorSlider_Loaded(object sender, RoutedEventArgs e)
        {
            // if the SelectedColor is not the default color we need to run the color calc mechanism
            if (SelectedColor != Colors.Red)
            {
                SetValueBasedOnColor(SelectedColor);
            }
        }

        /// <summary>Calculates the distance between 2 colors.</summary>
        /// <param name="source">The color that is being compared.</param>
        /// <param name="target">The target or desired color.</param>
        /// <returns>The distance between the 2 colors.</returns>
        protected virtual double Distance(Color source, Color target)
        {
            System.Drawing.Color c1 = System.Drawing.Color.FromArgb(source.R, source.G, source.B);
            System.Drawing.Color c2 = System.Drawing.Color.FromArgb(target.R, target.G, target.B);

            double hue = c1.GetHue() - c2.GetHue();
            double saturation = c1.GetSaturation() - c2.GetSaturation();
            double brightness = c1.GetBrightness() - c2.GetBrightness();

            return (hue * hue) + (saturation * saturation) + (brightness * brightness);
        }

        /// <summary>Gets the color at the associated position in the slider.</summary>
        /// <param name="position">The position of the thumb in the slider (pixel wise, not value wise).</param>
        /// <returns>The color at the specified position.</returns>
        protected virtual Color GetColorAtPosition(int position)
        {
            if (colorGradientImage == null) return Color.FromArgb(0, 0, 0, 0);

            CroppedBitmap cb;

            if (Orientation == Orientation.Horizontal)
            {
                // if the value is == the Width of the gradient image an ArgumentException is thrown, so we 
                // need to ensure the value stays just 1 pixel shy of that to avoid the exception
                if (position >= colorGradientImage.Width)
                    position = (int)colorGradientImage.Width - 1;

                cb = new CroppedBitmap(colorGradientImage, new Int32Rect(position, (int)colorGradientImage.Height / 2, 1, 1));
            }
            else
            {
                // if the value is == the Height of the gradient image an ArgumentException is thrown, so we 
                // need to ensure the value stays just 1 pixel shy of that to avoid the exception
                if (position >= colorGradientImage.Height)
                    position = (int)colorGradientImage.Height - 1;

                cb = new CroppedBitmap(colorGradientImage, new Int32Rect((int)colorGradientImage.Width / 2, position, 1, 1));
            }

            // create a 1 x 1 image to read the pixel color from
            byte[] rgb = new byte[4];

            cb.CopyPixels(rgb, 4, 0);

            // gen color from read pixel color
            Color returnColor = Color.FromRgb(rgb[2], rgb[1], rgb[0]);

            return returnColor;
        }

        public override void OnApplyTemplate()
        {
            colorGradient = GetTemplateChild("PART_ColorGradient") as Border;

            base.OnApplyTemplate();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            colorGradientImage = colorGradient.CaptureAsImage();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (colorGradient == null) return;
            if (isBeingUpdated) return; // update originated on set color

            int position;

            if (Orientation == Orientation.Horizontal)
            {
                position = (int)(newValue / (Maximum - Minimum) * VisualTreeHelper.GetDescendantBounds(colorGradient).Width);
            }
            else
            {
                // vertical sliders have their max value at the top (pixel 0 vertically) and min value at the bottom (pixel Height veritcally)
                // so we need the absolute position - Maximum instead to get the accurate color
                position = (int)(Math.Abs(newValue - Maximum) / (Maximum - Minimum) * VisualTreeHelper.GetDescendantBounds(colorGradient).Height);
            }

            isBeingUpdated = true;

            SelectedColor = GetColorAtPosition(position);

            isBeingUpdated = false;

            base.OnValueChanged(oldValue, newValue);
        }

        private static void SelectedColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSlider cs = d as ColorSlider;

            if (cs == null) return;

            Color c = (Color)e.NewValue;

            cs.SetValueBasedOnColor(c);
            cs.RaiseEvent(new RoutedEventArgs(SelectedColorChangedEvent));
        }

        /// <summary>Sets the value of the slider based on the specified color.</summary>
        /// <param name="color">The color to set on the slider.</param>
        protected virtual void SetValueBasedOnColor(Color color)
        {
            if (colorGradient == null) return;
            if (isBeingUpdated) return; // update originated on value change

            double currentDistance = int.MaxValue;
            int currentPos = -1;
            int upperBounds;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(colorGradient);

            // loop through the height (or width) of the image (checking every pixel) to see if we have a color match
            if (Orientation == Orientation.Horizontal)
            {
                upperBounds = (int)bounds.Width;
            }
            else
            {
                upperBounds = (int)bounds.Height;
            }

            for (int i = 0; i < upperBounds; i++)
            {
                Color temp = GetColorAtPosition(i);

                // determine distance between the two colors
                double dis = Distance(temp, color);

                if (dis == 0.0)
                {
                    currentPos = i;

                    break;
                }

                if (dis < currentDistance)
                {
                    currentDistance = dis;
                    currentPos = i;
                }
            }

            // update value based on calculate result
            isBeingUpdated = true;

            if (Orientation == Orientation.Horizontal)
            {
                // do not remove the double cast, they are not redundant, without them resulting division is integer and we get 0 not a decimal number
                Value = (double)currentPos / (double)upperBounds * (Maximum - Minimum);
            }
            else
            {
                // do not remove the double cast, they are not redundant, without them resulting division is integer and we get 0 not a decimal number
                Value = Math.Abs((double)currentPos / (double)upperBounds * (Maximum - Minimum) - Maximum);
            }

            isBeingUpdated = false;
        }

        #endregion
    }
}

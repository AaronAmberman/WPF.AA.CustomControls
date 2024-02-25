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
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Red, SelectedColorChanged));

        #endregion

        #region Constructors

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public ColorSlider()
        {
            Loaded += ColorSlider_Loaded;
        }

        #endregion

        #region Methods

        private void CacheColorGradientVisual()
        {
            if (colorGradient == null) return;

            Rect bounds = VisualTreeHelper.GetDescendantBounds(colorGradient);            
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen()) 
            {
                dc.DrawRectangle(new VisualBrush(colorGradient), null, new Rect(new Point(), bounds.Size));
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            colorGradientImage = rtb;
        }

        private void ColorSlider_Loaded(object sender, RoutedEventArgs e)
        {
            // if the SelectedColor is not the default color we need to run the color calc mechanism
            if (SelectedColor != Colors.Red)
            {
                SetValueBasedOnColor(SelectedColor);
            }
        }

        private double Distance(Color source, Color target)
        {
            System.Drawing.Color c1 = System.Drawing.Color.FromArgb(source.R, source.G, source.B);
            System.Drawing.Color c2 = System.Drawing.Color.FromArgb(target.R, target.G, target.B);

            double hue = c1.GetHue() - c2.GetHue();
            double saturation = c1.GetSaturation() - c2.GetSaturation();
            double brightness = c1.GetBrightness() - c2.GetBrightness();

            return (hue * hue) + (saturation * saturation) + (brightness * brightness);
        }

        private Color GetColorAtPosition(int position) 
        {
            if (colorGradientImage == null) return Color.FromArgb(0, 0, 0, 0);

            CroppedBitmap cb = new CroppedBitmap(colorGradientImage, new Int32Rect((int)colorGradientImage.Width / 2, position, 1, 1));
            byte[] rgb = new byte[4];

            cb.CopyPixels(rgb, 4, 0);

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

            CacheColorGradientVisual();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (colorGradient == null) return;
            if (isBeingUpdated) return; // update originated on set color

            // vertical sliders have their max value at the top (pixel 0 vertically) and min value at the bottom (pixel Height veritcally)
            int position = (int)(Math.Abs(newValue - 1000) / (Maximum - Minimum) * VisualTreeHelper.GetDescendantBounds(colorGradient).Height);

            isBeingUpdated = true;

            SelectedColor = GetColorAtPosition(position);

            isBeingUpdated = false;

            base.OnValueChanged(oldValue, newValue);
        }

        private static void SelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSlider cs = d as ColorSlider;

            if (cs == null) return;

            cs.SetValueBasedOnColor((Color)e.NewValue);
        }

        private void SetValueBasedOnColor(Color color)
        {
            if (colorGradient == null) return;
            if (isBeingUpdated) return; // update originated on value change

            double currentDistance = int.MaxValue;
            int currentPos = -1;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(colorGradient);

            for (int i = 0; i < bounds.Height; i++)
            {
                Color temp = GetColorAtPosition(i);
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

            isBeingUpdated = true;

            Value = Math.Abs((currentPos / bounds.Height) * (Maximum - Minimum) - 1000);

            isBeingUpdated = false;
        }

        #endregion
    }
}

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPF.AA.CustomControls.ColorSpace;

namespace WPF.AA.CustomControls
{
    public static class ExtensionMethods
    {
        #region Color

        /// <summary>Converts the color to the CMYK color space.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The CMYK equivalent.</returns>
        public static CMYK ToCmyk(this System.Drawing.Color color) 
        {
            return ColorSpace.ColorConverter.ConvertRgbToCmyk(color);
        }

        /// <summary>Converts the color to the HSL color space.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The HSL equivalent.</returns>
        public static HSL ToHsl(this System.Drawing.Color color)
        {
            return ColorSpace.ColorConverter.ConvertRgbToHsl(color);
        }

        /// <summary>Converts the color to the HSV color space.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The HSV equivalent.</returns>
        public static HSV ToHsv(this System.Drawing.Color color)
        {
            return ColorSpace.ColorConverter.ConvertRgbToHsv(color);
        }

        /// <summary>Converts the color to the CMYK color space.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The CMYK equivalent.</returns>
        public static CMYK ToCmyk(this Color color)
        {
            return ColorSpace.ColorConverter.ConvertRgbToCmyk(color);
        }

        /// <summary>Converts the color to the HSL color space.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The HSL equivalent.</returns>
        public static HSL ToHsl(this Color color)
        {
            return ColorSpace.ColorConverter.ConvertRgbToHsl(color);
        }

        /// <summary>Converts the color to the HSV color space.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The HSV equivalent.</returns>
        public static HSV ToHsv(this Color color)
        {
            return ColorSpace.ColorConverter.ConvertRgbToHsv(color);
        }

        #endregion

        #region Visual

        /// <summary>Captures the visual of the element and renders it as a BitmapSource.</summary>
        /// <param name="visual">The visual to capture as an image.</param>
        /// <returns>A BitmapSource for the given FrameworkElement.</returns>
        public static BitmapSource CaptureAsImage(this Visual visual) 
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(visual);
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(visual), null, new Rect(new Point(), bounds.Size));
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            return rtb;
        }

        #endregion
    }
}

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.AA.CustomControls
{
    public static class ExtensionMethods
    {
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF.AA.CustomControls
{
    /// <summary>A ListBox that has built in zooming capabilities.</summary>
    [TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = "PART_ResetZoom", Type = typeof(Button))]
    public class ZoomableListBox : ListBox, IDisposable
    {
        #region Fields

        private bool disposedValue;
        private ItemsPresenter itemsPresenter;
        private Button resetZoom;

        #endregion

        #region Properties

        /// <summary>Gets or sets the zoom factor for the ListBox. Default is 1.0.</summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ZoomableListBox), new PropertyMetadata(1.0, ZoomFactorChanged, CoerceZoomFactor));

        /// <summary>Gets or sets the max zoom factor. Default is 2.0.</summary>
        public double ZoomMax
        {
            get { return (double)GetValue(ZoomMaxProperty); }
            set { SetValue(ZoomMaxProperty, value); }
        }

        public static readonly DependencyProperty ZoomMaxProperty =
            DependencyProperty.Register("ZoomMax", typeof(double), typeof(ZoomableListBox), new PropertyMetadata(2.0));

        /// <summary>Gets or sets the min zoom factor. Default is 0.5.</summary>
        public double ZoomMin
        {
            get { return (double)GetValue(ZoomMinProperty); }
            set { SetValue(ZoomMinProperty, value); }
        }

        public static readonly DependencyProperty ZoomMinProperty =
            DependencyProperty.Register("ZoomMin", typeof(double), typeof(ZoomableListBox), new PropertyMetadata(0.5));

        #endregion

        #region Constructors

        static ZoomableListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomableListBox), new FrameworkPropertyMetadata(typeof(ZoomableListBox)));
        }

        public ZoomableListBox()
        {
            PreviewMouseWheel += ZoomableListView_PreviewMouseWheel;
        }

        #endregion

        #region Methods

        private static void ZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ZoomableListBox zlv) return;

            if (zlv.itemsPresenter != null)
                zlv.itemsPresenter.LayoutTransform = new ScaleTransform(zlv.ZoomFactor, zlv.ZoomFactor);
        }

        private static object CoerceZoomFactor(DependencyObject d, object baseValue)
        {
            if (d is not ZoomableListBox zlv) return baseValue;

            double val = (double)baseValue;

            if (val < zlv.ZoomMin) baseValue = zlv.ZoomMin;
            else if (val > zlv.ZoomMax) baseValue = zlv.ZoomMax;

            return baseValue;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            itemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
            resetZoom = GetTemplateChild("PART_ResetZoom") as Button;

            if (resetZoom != null)
                resetZoom.Click += ResetZoom_Click;
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            SetValue(ZoomFactorProperty, 1.0);
        }

        private void ZoomableListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta < 0)
            {
                SetValue(ZoomFactorProperty, ZoomFactor - 0.1);
            }
            else if (e.Delta > 0)
            {
                SetValue(ZoomFactorProperty, ZoomFactor + 0.1);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PreviewMouseWheel -= ZoomableListView_PreviewMouseWheel;

                    if (resetZoom != null)
                        resetZoom.Click -= ResetZoom_Click;
                }

                disposedValue=true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

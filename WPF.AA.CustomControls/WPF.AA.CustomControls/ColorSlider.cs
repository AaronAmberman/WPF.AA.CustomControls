using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF.AA.CustomControls.ColorSpace;

namespace WPF.AA.CustomControls
{
    /// <summary>A color slider control.</summary>
    public class ColorSlider : Slider
    {
        #region Fields

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
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Transparent, SelectedColorChangedCallback));

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
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // if we have a SelectedColor prior to having our template applied then we need to set the value to match
            if (SelectedColor != Colors.Transparent)
            {
                /*
                 * we use the HSV color space to simply get the hue value, our min should be 0 and our max should be 360, this way 
                 * the values match to the 360 degree hue angle of the HSV color space, this way it is simply a value mapping
                 */
                Value = SelectedColor.ToHsv().H;
            }
            else
            {
                // we do not want to load the default transparent, we do this here to trigger a value change rather than set the default to red
                Value = 0; // red is on the left
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (isBeingUpdated) return; // update originated on SelectedColor change, leave

            isBeingUpdated = true;

            /*
             * we use the HSV color space to simply get the hue value, our min should be 0 and our max should be 360, this way 
             * the values match to the 360 degree hue angle of the HSV color space, this way it is simply a value mapping
             */

            SelectedColor = new HSV { H = (float)newValue, S = 1, V = 1 }.ToMediaColor();

            isBeingUpdated = false;
        }

        private static void SelectedColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSlider cs = d as ColorSlider;

            if (cs == null) return;
            if (cs.isBeingUpdated) return; // update originated on value change, leave

            Color c = (Color)e.NewValue;

            /*
             * we use the HSV color space to simply get the hue value, our min should be 0 and our max should be 360, this way 
             * the values match to the 360 degree hue angle of the HSV color space, this way it is simply a value mapping
             */

            cs.isBeingUpdated = true;

            cs.Value = c.ToHsv().H;

            cs.isBeingUpdated = false;
            cs.RaiseEvent(new RoutedEventArgs(SelectedColorChangedEvent));
        }

        #endregion
    }
}

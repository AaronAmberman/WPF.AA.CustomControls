using System.Windows;
using System.Windows.Controls;

namespace WPF.AA.CustomControls
{
    /// <summary>An Expander that presents its content in a Popup.</summary>
    public class PopupExpander : Expander
    {
        #region Properties

        /// <summary>Gets or sets the corner radius for the expander.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PopupExpander), new PropertyMetadata(null));

        /// <summary>Gets or sets the horizontal offset for the popup.</summary>
        public double PopupHorizontalOffset
        {
            get { return (double)GetValue(PopupHorizontalOffsetProperty); }
            set { SetValue(PopupHorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty PopupHorizontalOffsetProperty =
            DependencyProperty.Register("PopupHorizontalOffset", typeof(double), typeof(PopupExpander), new PropertyMetadata(0.0));

        /// <summary>Gets or sets the placement rectangle for the popup portion of the control.</summary>
        public Rect PopupPlacementRectangle
        {
            get { return (Rect)GetValue(PopupPlacementRectangleProperty); }
            set { SetValue(PopupPlacementRectangleProperty, value); }
        }

        public static readonly DependencyProperty PopupPlacementRectangleProperty =
            DependencyProperty.Register("PopupPlacementRectangle", typeof(Rect), typeof(PopupExpander), new PropertyMetadata(Rect.Empty));

        /// <summary>Gets or sets whether or not the popup stays open or not. Default is false.</summary>
        public bool PopupStaysOpen
        {
            get { return (bool)GetValue(PopupStaysOpenProperty); }
            set { SetValue(PopupStaysOpenProperty, value); }
        }

        public static readonly DependencyProperty PopupStaysOpenProperty =
            DependencyProperty.Register("PopupStaysOpen", typeof(bool), typeof(PopupExpander), new PropertyMetadata(false));

        /// <summary>Gets or sets the vertical offset for the popup.</summary>
        public double PopupVerticalOffset
        {
            get { return (double)GetValue(PopupVerticalOffsetProperty); }
            set { SetValue(PopupVerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty PopupVerticalOffsetProperty =
            DependencyProperty.Register("PopupVerticalOffset", typeof(double), typeof(PopupExpander), new PropertyMetadata(0.0));

        #endregion

        #region Constructors

        static PopupExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupExpander), new FrameworkPropertyMetadata(typeof(PopupExpander)));
        }

        #endregion
    }
}

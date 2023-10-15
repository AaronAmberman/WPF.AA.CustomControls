using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPF.AA.CustomControls
{
    /// <summary>A normal ToggleButton with a CornerRadius Property.</summary>
    public class RoundableToggleButton : ToggleButton
    {
        #region Properties

        /// <summary>Gets or sets the corner radius for the button.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RoundableToggleButton), new PropertyMetadata(null));

        #endregion

        #region Constructors

        static RoundableToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundableToggleButton), new FrameworkPropertyMetadata(typeof(RoundableToggleButton)));
        }

        #endregion
    }
}

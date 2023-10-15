using System.Windows;
using System.Windows.Controls;

namespace WPF.AA.CustomControls
{
    /// <summary>A normal Button with a CornerRadius Property.</summary>
    public class RoundableButton : Button
    {
        #region Properties

        /// <summary>Gets or sets the corner radius for the button.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RoundableButton), new PropertyMetadata(null));

        #endregion

        #region Constructors

        static RoundableButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundableButton), new FrameworkPropertyMetadata(typeof(RoundableButton)));
        }

        #endregion
    }
}

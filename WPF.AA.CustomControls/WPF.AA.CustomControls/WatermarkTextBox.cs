using System.Windows;
using System.Windows.Controls;

namespace WPF.AA.CustomControls
{
    /// <summary>A TextBox that provides a watermark overlay.</summary>
    public class WatermarkTextBox : TextBox
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>Gets or sets the watermark.</summary>
        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new PropertyMetadata(null));

        /// <summary>Gets or sets the watermark template.</summary>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkTextBox), new PropertyMetadata(null));

        #endregion

        #region Constructors

        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

        #endregion

        #region Methods

        #endregion
    }
}

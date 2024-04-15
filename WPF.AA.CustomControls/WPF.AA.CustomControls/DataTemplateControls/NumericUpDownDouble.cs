using System.Windows;

namespace WPF.AA.CustomControls.DataTemplateControls
{
    public class NumericUpDownDouble : NumericUpDown
    {
        #region Constructors

        static NumericUpDownDouble()
        {
            ValueTypeProperty.OverrideMetadata(typeof(NumericUpDownDouble), new PropertyMetadata(NumericUpDownType.Double));
        }

        #endregion
    }
}

using System.Windows;

namespace WPF.AA.CustomControls.DataTemplateControls
{
    public class NumericUpDownDecimal : NumericUpDown
    {
        #region Constructors

        static NumericUpDownDecimal()
        {
            ValueTypeProperty.OverrideMetadata(typeof(NumericUpDownDecimal), new PropertyMetadata(NumericUpDownType.Decimal));
        }

        #endregion
    }
}

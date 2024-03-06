using System.Windows.Media;

namespace WPF.AA.CustomControls.ColorSpace
{
    public class HSL
    {
        #region Properties

        /// <summary>Gets or set the H value. Value range should be 0 to 360 (not enforced).</summary>
        public float H { get; set; }

        /// <summary>Gets or set the S value. Value range should be 0 to 1 (not enforced).</summary>
        public float S { get; set; }

        /// <summary>Gets or set the L value. Value range should be 0 to 1 (not enforced).</summary>
        public float L { get; set; }

        #endregion

        #region Methods

        /// <summary>Converts the HSL to a CMYK value.</summary>
        /// <returns>The CMYK equivalent.</returns>
        public CMYK ToCmyk()
        {
            return ColorConverter.ConvertHslToCmyk(this);
        }

        /// <summary>Converts the HSL to a <see cref="System.Drawing.Color"/> value.</summary>
        /// <returns>The RGB equivalent.</returns>
        public System.Drawing.Color ToDrawingColor()
        {
            return ColorConverter.ConvertHslToRgbDrawing(this);
        }

        /// <summary>Converts the HSL to a HSV value.</summary>
        /// <returns>The HSV equivalent.</returns>
        public HSV ToHsv()
        {
            return ColorConverter.ConvertHslToHsv(this);
        }

        /// <summary>Converts the HSL to a <see cref="System.Windows.Media.Color"/> value.</summary>
        /// <returns>The RGB equivalent.</returns>
        public Color ToMediaColor()
        {
            return ColorConverter.ConvertHslToRgb(this);
        }

        #endregion
    }
}

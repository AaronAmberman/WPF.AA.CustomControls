using System.Windows.Media;

namespace WPF.AA.CustomControls.ColorSpace
{
    public class HSV
    {
        #region Properties

        /// <summary>Gets or set the H value. Value range should be 0 to 360 (not enforced).</summary>
        public float H { get; set; }

        /// <summary>Gets or set the S value. Value range should be 0 to 1 (not enforced).</summary>
        public float S { get; set; }

        /// <summary>Gets or set the V value. Value range should be 0 to 1 (not enforced).</summary>
        public float V { get; set; }

        #endregion

        #region Methods

        /// <summary>Converts the HSV to a CMYK value.</summary>
        /// <returns>The CMYK equivalent.</returns>
        public CMYK ToCmyk()
        {
            return ColorConverter.ConvertHsvToCmyk(this);
        }

        /// <summary>Converts the HSV to a <see cref="System.Drawing.Color"/> value.</summary>
        /// <returns>The RGB equivalent.</returns>
        public System.Drawing.Color ToDrawingColor()
        {
            return ColorConverter.ConvertHsvToRgbDrawing(this);
        }

        /// <summary>Converts the HSV to a HSL value.</summary>
        /// <returns>The HSL equivalent.</returns>
        public HSL ToHsl()
        {
            return ColorConverter.ConvertHsvToHsl(this);
        }

        /// <summary>Converts the HSV to a <see cref="System.Windows.Media.Color"/> value.</summary>
        /// <returns>The RGB equivalent.</returns>
        public Color ToMediaColor()
        {
            return ColorConverter.ConvertHsvToRgb(this);
        }

        #endregion
    }
}

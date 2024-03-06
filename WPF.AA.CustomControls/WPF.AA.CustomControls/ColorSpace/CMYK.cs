using System.Windows.Media;

namespace WPF.AA.CustomControls.ColorSpace
{
    public class CMYK
    {
        #region Properties

        /// <summary>Gets or sets the C value. Value range should be 0 to 100 (not enforced).</summary>
        public byte C { get; set; }

        /// <summary>Gets or sets the M value. Value range should be 0 to 100 (not enforced).</summary>
        public byte M { get; set; }

        /// <summary>Gets or sets the Y value. Value range should be 0 to 100 (not enforced).</summary>
        public byte Y { get; set; }

        /// <summary>Gets or sets the K value. Value range should be 0 to 100 (not enforced).</summary>
        public byte K { get; set; }

        #endregion

        #region Methods

        /// <summary>Converts the CMYK to a <see cref="System.Drawing.Color"/> RGB value.</summary>
        /// <returns>The RGB equivalent.</returns>
        public System.Drawing.Color ToDrawingColor()
        {
            return ColorConverter.ConvertCmykToRgbDrawing(this);
        }

        /// <summary>Converts the CMYK to a HSL value.</summary>
        /// <returns>The HSL equivalent.</returns>
        public HSL ToHsl()
        {
            return ColorConverter.ConvertCmykToHsl(this);
        }

        /// <summary>Converts the CMYK to a HSV value.</summary>
        /// <returns>The HSV equivalent.</returns>
        public HSV ToHsv()
        {
            return ColorConverter.ConvertCmykToHsv(this);
        }

        /// <summary>Converts the CMYK to a <see cref="System.Windows.Media.Color"/> RGB value.</summary>
        /// <returns>The RGB equivalent.</returns>
        public Color ToMediaColor()
        {
            return ColorConverter.ConvertCmykToRgb(this);
        }

        #endregion
    }
}

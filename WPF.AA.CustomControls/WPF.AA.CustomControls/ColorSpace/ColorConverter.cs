using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace WPF.AA.CustomControls.ColorSpace
{
    /// <summary>Helps convert colors between different color spaces.</summary>
    public static class ColorConverter
    {
        // https://www.easyrgb.com/en/math.php#text20
        // https://github.com/xceedsoftware/wpftoolkit/blob/master/ExtendedWPFToolkitSolution/Src/Xceed.Wpf.Toolkit/Core/Utilities/ColorUtilities.cs
        // https://stackoverflow.com/questions/359612/how-to-convert-rgb-color-to-hsv
        // https://bootcamp.uxdesign.cc/definitive-guide-to-wpf-colors-color-spaces-color-pickers-and-creating-your-own-colors-for-mere-f480935c6e94

        #region Methods

        #region Convert Between Media.Color and Drawing.Color

        /// <summary>Converts a <see cref="System.Drawing.Color"/> to a <see cref="System.Windows.Media.Color"/>.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color.</returns>
        public static Color ConvertDrawingColorToMediaColor(System.Drawing.Color color)
        {
            return new Color
            {
                A = color.A,
                B = color.B,
                G = color.G,
                R = color.R
            };
        }

        /// <summary>Converts a <see cref="System.Windows.Media.Color"/> to a <see cref="System.Drawing.Color"/>.</summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color.</returns>
        public static System.Drawing.Color ConvertMediaColorToDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion

        #region CMYK

        /// <summary>Converts a color in the CMYK space to the HSL space.</summary>
        /// <param name="cmyk">The CMYK (CMYK values are expressed 1 to 100).</param>
        /// <returns>The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1) equivalent.</returns>
        public static HSL ConvertCmykToHsl(CMYK cmyk)
        {
            return ConvertRgbToHsl(ConvertCmykToRgbDrawing(cmyk));
        }

        /// <summary>Converts a color in the CMYK space to the HSV space.</summary>
        /// <param name="cmyk">The CMYK (CMYK values are expressed 1 to 100).</param>
        /// <returns>The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1) equivalent.</returns>
        public static HSV ConvertCmykToHsv(CMYK cmyk)
        {
            return ConvertRgbToHsv(ConvertCmykToRgbDrawing(cmyk));
        }

        /// <summary>Converts a color in the CMYK space to the RGB space.</summary>
        /// <param name="cmyk">The CMYK (CMYK values are expressed 1 to 100).</param>
        /// <returns>The RGB equivalent.</returns>
        public static Color ConvertCmykToRgb(CMYK cmyk)
        {
            return ConvertDrawingColorToMediaColor(ConvertCmykToRgbDrawing(cmyk));
        }

        /// <summary>Converts a color in the CMYK space to the RGB space.</summary>
        /// <param name="cmyk">The CMYK (CMYK values are expressed 1 to 100).</param>
        /// <returns>The RGB equivalent.</returns>
        public static System.Drawing.Color ConvertCmykToRgbDrawing(CMYK cmyk)
        {
            return System.Drawing.Color.FromArgb(255,
                (byte)Math.Round(255 * (1 - cmyk.C * 0.01) * (1 - cmyk.K * 0.01)),
                (byte)Math.Round(255 * (1 - cmyk.M * 0.01) * (1 - cmyk.K * 0.01)),
                (byte)Math.Round(255 * (1 - cmyk.Y * 0.01) * (1 - cmyk.K * 0.01)));
        }

        #endregion

        #region HSL

        /// <summary>Converts a color in the HSL space to the CMYK space.</summary>
        /// <param name="hsl">The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1).</param>
        /// <returns>The CMYK equivalent (CMYK values are expressed 1 to 100).</returns>
        public static CMYK ConvertHslToCmyk(HSL hsl)
        {
            return ConvertRgbToCmyk(ConvertHslToRgbDrawing(hsl));
        }

        /// <summary>Converts a color in the HSL space to the HSV space.</summary>
        /// <param name="hsl">The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1).</param>
        /// <returns>The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1) equivalent.</returns>
        public static HSV ConvertHslToHsv(HSL hsl)
        {
            float hsvV = hsl.L + hsl.S * Math.Min(hsl.L, 1 - hsl.L);
            float hsvS = (hsvV == 0) ? 0 : 2 * (1 - hsl.L / hsvV);

            return new HSV
            {
                H = hsl.H, 
                S = hsvS, 
                V = hsvV 
            };
        }

        /// <summary>Converts a color in the HSL space to the RGB space.</summary>
        /// <param name="hsl">The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1).</param>
        /// <returns>The RGB equivalent.</returns>
        public static Color ConvertHslToRgb(HSL hsl)
        {
            return ConvertDrawingColorToMediaColor(ConvertHslToRgbDrawing(hsl));
        }

        /// <summary>Converts a color in the HSL space to the RGB space.</summary>
        /// <param name="hsl">The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1).</param>
        /// <returns>The RGB equivalent.</returns>
        public static System.Drawing.Color ConvertHslToRgbDrawing(HSL hsl)
        {
            float hue, r, g, b, max, min;

            hue = hsl.H / 360.0f;
            max = (hsl.L < 0.5) ? hsl.L * (1.0f + hsl.S) : hsl.L + hsl.S - (hsl.L * hsl.S);
            min = 2.0f * hsl.L - max;

            if (hsl.L == 0)  // 0 is always be black
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else if (hsl.S != 0)
            {
                r = GetComponentHue(min, max, hue + 1.0f / 3.0f);
                g = GetComponentHue(min, max, hue);
                b = GetComponentHue(min, max, hue - 1.0f / 3.0f);
            }
            else // ensure greys are not converted to white
            {
                r = hsl.L;
                g = hsl.L;
                b = hsl.L;
            }

            return System.Drawing.Color.FromArgb(255, (byte)Math.Round(r * 255), (byte)Math.Round(g * 255), (byte)Math.Round(b * 255));
        }

        private static float GetComponentHue(float min, float max, float hue)
        {
            float value = min;

            if (hue < 0.0f) hue++;
            if (hue > 1.0f) hue--;
            
            if (hue * 6.0f < 1.0f)
                value = min + (max - min) * 6.0f * hue;
            else if (hue * 2.0f < 1.0f)
                value = max;
            else if (hue * 3.0f < 2.0f)
                value = min + (max - min) * (2.0f / 3.0f - hue) * 6.0f;

            return value;
        }

        #endregion

        #region HSV

        /// <summary>Converts a color in the HSV space to the CMYK space.</summary>
        /// <param name="hsv">The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1).</param>
        /// <returns>The CMYK equivalent (CMYK values are expressed 1 to 100).</returns>
        public static CMYK ConvertHsvToCmyk(HSV hsv)
        {
            return ConvertHslToCmyk(ConvertHsvToHsl(hsv));
        }

        /// <summary>Converts a color in the HSV space to the HSL space.</summary>
        /// <param name="hsv">The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1).</param>
        /// <returns>The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1) equivalent.</returns>
        public static HSL ConvertHsvToHsl(HSV hsv)
        {
            float hslL = hsv.V * (1 - hsv.S / 2);
            float hslS = (hslL == 0 || hslL == 1) ? 0 : (hsv.V - hslL) / Math.Min(hslL, 1 - hslL);

            return new HSL
            {
                H = hsv.H, 
                S = hslS, 
                L = hslL
            };
        }

        /// <summary>Converts a color in the HSV space to the RGB space.</summary>
        /// <param name="hsv">The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1).</param>
        /// <returns>The RGB equivalent.</returns>
        public static Color ConvertHsvToRgb(HSV hsv)
        {
            return ConvertDrawingColorToMediaColor(ConvertHsvToRgbDrawing(hsv));
        }

        /// <summary>Converts a color in the HSV space to the RGB space.</summary>
        /// <param name="hsv">The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1).</param>
        /// <returns>The RGB equivalent.</returns>
        public static System.Drawing.Color ConvertHsvToRgbDrawing(HSV hsv)
        {
            if (hsv.S == 0)
            {
                byte val = (byte)(hsv.V * 255);
                return System.Drawing.Color.FromArgb(255, val, val, val);
            }
            else if (hsv.V <= 0)
                return System.Drawing.Color.FromArgb(255, 0, 0, 0);

            int hi = Convert.ToInt32(Math.Floor(hsv.H / 60)) % 6;
            float f = hsv.H / 60 - (float)Math.Floor(hsv.H / 60);

            float value = hsv.V * 255;

            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - hsv.S));
            byte q = Convert.ToByte(value * (1 - f * hsv.S));
            byte t = Convert.ToByte(value * (1 - (1 - f) * hsv.S));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, t, p, v);
            else
                return System.Drawing.Color.FromArgb(255, v, p, q);
        }

        #endregion

        #region RGB

        /// <summary>Converts a color in the RGB space to the CMYK space.</summary>
        /// <param name="color">The RGB color.</param>
        /// <returns>The CMYK equivalent (CMYK values are expressed 1 to 100).</returns>
        public static CMYK ConvertRgbToCmyk(Color color)
        {
            return ConvertRgbToCmyk(ConvertMediaColorToDrawingColor(color));
        }

        /// <summary>Converts a color in the RGB space to the CMYK space.</summary>
        /// <param name="color">The RGB color.</param>
        /// <returns>The CMYK equivalent (CMYK values are expressed 1 to 100).</returns>
        public static CMYK ConvertRgbToCmyk(System.Drawing.Color color)
        {
            double r, g, b, c, m, y, k;

            r = color.R / 255.0;
            g = color.G / 255.0;
            b = color.B / 255.0;

            k = 1 - new List<double>() { r, g, b }.Max();
            c = (1 - r - k) / (1 - k);
            m = (1 - g - k) / (1 - k);
            y = (1 - b - k) / (1 - k);

            return new CMYK
            {
                C = (byte)Math.Round(c * 100),
                M = (byte)Math.Round(m * 100),
                Y = (byte)Math.Round(y * 100),
                K = (byte)Math.Round(k * 100)
            };
        }

        /// <summary>Converts a color in the RGB space to the HSL space.</summary>
        /// <param name="color">The RGB color.</param>
        /// <returns>The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1) equivalent.</returns>
        public static HSL ConvertRgbToHsl(Color color) 
        {
            return ConvertRgbToHsl(ConvertMediaColorToDrawingColor(color));
        }

        /// <summary>Converts a color in the RGB space to the HSL space.</summary>
        /// <param name="color">The RGB color.</param>
        /// <returns>The HSL (hsl values; H = 0 - 360, S = 0 - 1, L = 0 - 1) equivalent.</returns>
        public static HSL ConvertRgbToHsl(System.Drawing.Color color)
        {
            return new HSL
            {
                H = color.GetHue(),
                S = color.GetSaturation(),
                L = color.GetBrightness()
            };
        }

        /// <summary>Converts a color in the RGB space to the HSV space.</summary>
        /// <param name="color">The RGB color.</param>
        /// <returns>The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1) equivalent.</returns>
        public static HSV ConvertRgbToHsv(Color color)
        {
            return ConvertRgbToHsv(ConvertMediaColorToDrawingColor(color));
        }

        /// <summary>Converts a color in the RGB space to the HSV space.</summary>
        /// <param name="color">The RGB color.</param>
        /// <returns>The HSV (hsv values; H = 0 - 360, S = 0 - 1, V = 0 - 1) equivalent.</returns>
        public static HSV ConvertRgbToHsv(System.Drawing.Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            float hue = color.GetHue();
            float saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            float value = max / 255f;

            return new HSV
            {
                H = hue,
                S = saturation,
                V = value
            };
        }

        #endregion

        #endregion
    }
}

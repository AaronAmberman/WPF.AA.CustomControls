using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WPF.AA.CustomControls.ColorSpace
{
    /// <summary>Helps convert colors between different color spaces.</summary>
    public static class ColorConverter
    {
        // https://www.easyrgb.com/en/math.php
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
                byte val = (byte)(hsv.V * 255f);
                return System.Drawing.Color.FromArgb(255, val, val, val);
            }
            else if (hsv.V <= 0)
                return System.Drawing.Color.FromArgb(255, 0, 0, 0);

            float h = (hsv.H / 360f) * 6f; // our HSVs go form 0 to 360 not 0 to 1

            if (h == 6f) h = 0f;

            int i = (int)h;

            float one = hsv.V * (1 - hsv.S);
            float two = hsv.V * (1 - hsv.S * (h - i));
            float three = hsv.V * (1 - hsv.S * (1 -  (h - i)));

            float r, g, b;

            if (i == 0)
            {
                r = hsv.V;
                g = three;
                b = one;
            }
            else if (i == 1)
            {
                r = two;
                g = hsv.V;
                b = one;
            }
            else if (i == 2)
            {
                r = one;
                g = hsv.V;
                b = three;
            }
            else if (i == 3)
            {
                r = one;
                g = two;
                b = hsv.V;
            }
            else if (i == 4)
            {
                r = three;
                g = one;
                b = hsv.V;
            }
            else
            {
                r = hsv.V;
                g = one;
                b = two;
            }

            int convertedR = (int)(r * 255f);
            int convertedG = (int)(g * 255f);
            int convertedB = (int)(b * 255f);

            return System.Drawing.Color.FromArgb(255, convertedR, convertedG, convertedB);
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
            float minifiedR = color.R / 255f;
            float minifiedG = color.G / 255f;
            float minifiedB = color.B / 255f;

            float min = Math.Min(Math.Min(minifiedR, minifiedG), minifiedB);
            float max = Math.Max(Math.Max(minifiedR, minifiedG), minifiedB);
            float delta = max - min;

            float H = 0;
            float S = 0;
            float V = max;

            if (delta == 0) // gray
            {
                H = 0;
                S = 0;
            }
            else // color
            {
                S = delta / max;

                float deltaR = (((max - minifiedR) / 6) + (delta / 2)) / delta;
                float deltaG = (((max - minifiedG) / 6) + (delta / 2)) / delta;
                float deltaB = (((max - minifiedB) / 6) + (delta / 2)) / delta;

                if (minifiedR == max) H = deltaB - deltaG;
                else if (minifiedG == max) H = (1 / 3) + deltaR - deltaB;
                else if (minifiedB == max) H = (2 / 3) + deltaG - deltaR;

                if (H < 0) H += 1;
                if (H > 1) H -= 1;
            }

            return new HSV { H = H, S = S, V = V };
        }

        #endregion

        #endregion
    }
}
